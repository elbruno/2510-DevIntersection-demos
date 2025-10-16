using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TodoistMcpConsole.Services;

/// <summary>
/// Client for communicating with MCP (Model Context Protocol) servers via JSON-RPC over stdio.
/// </summary>
public class McpClient : IAsyncDisposable, IDisposable
{
    private readonly Process _process;
    private readonly StreamWriter _stdin;
    private readonly StreamReader _stdout;
    private readonly StreamReader _stderr;
    private int _messageId = 0;
    private readonly int _requestTimeout;
    private readonly int _maxRetries;
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the McpClient.
    /// </summary>
    /// <param name="process">The MCP server process.</param>
    /// <param name="stdin">Standard input stream for the process.</param>
    /// <param name="stdout">Standard output stream for the process.</param>
    /// <param name="stderr">Standard error stream for the process.</param>
    /// <param name="requestTimeout">Timeout for requests in seconds.</param>
    /// <param name="maxRetries">Maximum number of retries for failed requests.</param>
    private McpClient(Process process, StreamWriter stdin, StreamReader stdout, StreamReader stderr, int requestTimeout, int maxRetries)
    {
        _process = process;
        _stdin = stdin;
        _stdout = stdout;
        _stderr = stderr;
        _requestTimeout = requestTimeout;
        _maxRetries = maxRetries;
    }

    /// <summary>
    /// Creates and connects to an MCP server.
    /// </summary>
    /// <param name="serverPath">Path to the MCP server executable.</param>
    /// <param name="serverArgs">Arguments for the MCP server.</param>
    /// <param name="apiToken">API token for authentication.</param>
    /// <param name="requestTimeout">Timeout for requests in seconds.</param>
    /// <param name="maxRetries">Maximum number of retries for failed requests.</param>
    /// <returns>A connected McpClient instance.</returns>
    public static async Task<McpClient> ConnectAsync(string serverPath, string serverArgs, string apiToken, int requestTimeout = 60, int maxRetries = 3)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = serverPath,
            Arguments = serverArgs,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        // Set the API token as an environment variable
        processStartInfo.Environment["TODOIST_API_TOKEN"] = apiToken;

        var process = Process.Start(processStartInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start MCP server process.");
        }

        var stdin = process.StandardInput;
        var stdout = process.StandardOutput;
        var stderr = process.StandardError;

        var client = new McpClient(process, stdin, stdout, stderr, requestTimeout, maxRetries);

        // Initialize the connection
        await client.InitializeAsync();

        return client;
    }

    /// <summary>
    /// Initializes the MCP connection.
    /// </summary>
    private async Task InitializeAsync()
    {
        var initRequest = new
        {
            jsonrpc = "2.0",
            id = GetNextMessageId(),
            method = "initialize",
            @params = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new { },
                clientInfo = new
                {
                    name = "TodoistMcpConsole",
                    version = "1.0.0"
                }
            }
        };

        var response = await SendRequestAsync<JObject>(initRequest);
        
        // Send initialized notification
        var initializedNotification = new
        {
            jsonrpc = "2.0",
            method = "notifications/initialized"
        };

        await SendNotificationAsync(initializedNotification);
    }

    /// <summary>
    /// Lists available tools from the MCP server.
    /// </summary>
    /// <returns>A list of available tool names.</returns>
    public async Task<List<string>> ListToolsAsync()
    {
        var request = new
        {
            jsonrpc = "2.0",
            id = GetNextMessageId(),
            method = "tools/list",
            @params = new { }
        };

        var response = await SendRequestAsync<JObject>(request);
        var tools = response["result"]?["tools"] as JArray;
        
        if (tools == null)
        {
            return new List<string>();
        }

        return tools.Select(t => t["name"]?.ToString() ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList();
    }

    /// <summary>
    /// Calls a tool on the MCP server.
    /// </summary>
    /// <param name="toolName">Name of the tool to call.</param>
    /// <param name="arguments">Arguments for the tool.</param>
    /// <returns>The response from the tool invocation.</returns>
    public async Task<JObject> CallToolAsync(string toolName, object arguments)
    {
        var request = new
        {
            jsonrpc = "2.0",
            id = GetNextMessageId(),
            method = "tools/call",
            @params = new
            {
                name = toolName,
                arguments
            }
        };

        return await SendRequestAsync<JObject>(request);
    }

    /// <summary>
    /// Sends a JSON-RPC request and waits for response with retry logic.
    /// </summary>
    private async Task<T> SendRequestAsync<T>(object request)
    {
        int retries = 0;
        Exception? lastException = null;

        while (retries < _maxRetries)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                await _stdin.WriteLineAsync(json);
                await _stdin.FlushAsync();

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_requestTimeout));
                var responseLine = await _stdout.ReadLineAsync(cts.Token);

                if (string.IsNullOrEmpty(responseLine))
                {
                    throw new InvalidOperationException("Received empty response from MCP server.");
                }

                var response = JsonConvert.DeserializeObject<T>(responseLine);
                if (response == null)
                {
                    throw new InvalidOperationException("Failed to deserialize response from MCP server.");
                }

                return response;
            }
            catch (Exception ex) when (retries < _maxRetries - 1)
            {
                lastException = ex;
                retries++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retries))); // Exponential backoff
            }
        }

        throw new InvalidOperationException($"Failed to communicate with MCP server after {_maxRetries} retries.", lastException);
    }

    /// <summary>
    /// Sends a JSON-RPC notification (no response expected).
    /// </summary>
    private async Task SendNotificationAsync(object notification)
    {
        var json = JsonConvert.SerializeObject(notification);
        await _stdin.WriteLineAsync(json);
        await _stdin.FlushAsync();
    }

    /// <summary>
    /// Gets the next message ID for JSON-RPC requests.
    /// </summary>
    private int GetNextMessageId()
    {
        return Interlocked.Increment(ref _messageId);
    }

    /// <summary>
    /// Disposes of the MCP client and closes the connection asynchronously.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await _stdin.DisposeAsync();
        _stdout?.Dispose();
        _stderr?.Dispose();

        if (_process != null && !_process.HasExited)
        {
            _process.Kill();
            await _process.WaitForExitAsync();
        }

        _process?.Dispose();
        _disposed = true;
    }

    /// <summary>
    /// Disposes of the MCP client and closes the connection.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _stdin?.Dispose();
        _stdout?.Dispose();
        _stderr?.Dispose();

        if (_process != null && !_process.HasExited)
        {
            _process.Kill();
            _process.WaitForExit(5000);
        }

        _process?.Dispose();
        _disposed = true;
    }
}
