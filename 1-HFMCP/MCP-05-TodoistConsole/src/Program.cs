using System.CommandLine;
using Microsoft.Extensions.Configuration;
using TodoistMcpConsole.Commands;
using TodoistMcpConsole.Services;

namespace TodoistMcpConsole;

/// <summary>
/// Main program entry point for the Todoist MCP Console application.
/// </summary>
class Program
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code.</returns>
    static async Task<int> Main(string[] args)
    {
        // Load configuration from multiple sources
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>(optional: true)
            .Build();

        // Display welcome banner
        DisplayWelcomeBanner();

        // Get API token from configuration
        var apiToken = configuration["TODOIST_API_TOKEN"] 
                      ?? configuration["TodoistApiToken"]
                      ?? Environment.GetEnvironmentVariable("TODOIST_API_TOKEN");

        if (string.IsNullOrWhiteSpace(apiToken))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: TODOIST_API_TOKEN not found!");
            Console.WriteLine("\nPlease set your Todoist API token using one of the following methods:");
            Console.WriteLine("  1. User Secrets: dotnet user-secrets set \"TODOIST_API_TOKEN\" \"your-token\"");
            Console.WriteLine("  2. Environment Variable: export TODOIST_API_TOKEN=your-token");
            Console.WriteLine("  3. appsettings.json: Add \"TodoistApiToken\": \"your-token\"");
            Console.ResetColor();
            return 1;
        }

        // Get MCP server configuration
        var serverPath = configuration["McpServer:TodoistServerPath"] ?? "npx";
        var serverArgs = configuration["McpServer:TodoistServerArgs"] ?? "-y @doist/todoist-ai-mcp-server";
        var requestTimeout = int.Parse(configuration["McpServer:RequestTimeout"] ?? "60");
        var maxRetries = int.Parse(configuration["McpServer:MaxRetries"] ?? "3");

        // Create root command
        var rootCommand = new RootCommand("Todoist MCP Console - Manage your Todoist tasks via MCP");

        // List command
        var listCommand = new Command("list", "List all tasks from Todoist");
        listCommand.SetHandler(async () =>
        {
            await using var mcpClient = await McpClient.ConnectAsync(serverPath, serverArgs, apiToken, requestTimeout, maxRetries);
            var command = new ListCommand(mcpClient);
            await command.ExecuteAsync();
        });
        rootCommand.AddCommand(listCommand);

        // Add command
        var addCommand = new Command("add", "Add a new task to Todoist");
        var titleArgument = new Argument<string>("title", "The title/content of the task");
        addCommand.AddArgument(titleArgument);
        addCommand.SetHandler(async (string title) =>
        {
            await using var mcpClient = await McpClient.ConnectAsync(serverPath, serverArgs, apiToken, requestTimeout, maxRetries);
            var command = new AddCommand(mcpClient);
            await command.ExecuteAsync(title);
        }, titleArgument);
        rootCommand.AddCommand(addCommand);

        // Complete command
        var completeCommand = new Command("complete", "Mark a task as complete");
        var idArgument = new Argument<string>("id", "The ID of the task to complete");
        completeCommand.AddArgument(idArgument);
        completeCommand.SetHandler(async (string id) =>
        {
            await using var mcpClient = await McpClient.ConnectAsync(serverPath, serverArgs, apiToken, requestTimeout, maxRetries);
            var command = new CompleteCommand(mcpClient);
            await command.ExecuteAsync(id);
        }, idArgument);
        rootCommand.AddCommand(completeCommand);

        // Tools command (list available tools)
        var toolsCommand = new Command("tools", "List available MCP tools");
        toolsCommand.SetHandler(async () =>
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Connecting to Todoist MCP server...");
                Console.ResetColor();

                await using var mcpClient = await McpClient.ConnectAsync(serverPath, serverArgs, apiToken, requestTimeout, maxRetries);
                var tools = await mcpClient.ListToolsAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n=== Available MCP Tools ===");
                Console.ResetColor();

                foreach (var tool in tools)
                {
                    Console.WriteLine($"  • {tool}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error listing tools: {ex.Message}");
                Console.ResetColor();
            }
        });
        rootCommand.AddCommand(toolsCommand);

        try
        {
            return await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    /// <summary>
    /// Displays a welcome banner.
    /// </summary>
    private static void DisplayWelcomeBanner()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║     Todoist MCP Console v1.0.0         ║");
        Console.WriteLine("║   Manage tasks with MCP integration    ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }
}
