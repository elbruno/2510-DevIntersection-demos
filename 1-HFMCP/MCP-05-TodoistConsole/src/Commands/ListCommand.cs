using Newtonsoft.Json.Linq;
using TodoistMcpConsole.Services;

namespace TodoistMcpConsole.Commands;

/// <summary>
/// Command to list all tasks from Todoist.
/// </summary>
public class ListCommand
{
    private readonly McpClient _mcpClient;

    /// <summary>
    /// Initializes a new instance of the ListCommand.
    /// </summary>
    /// <param name="mcpClient">The MCP client to use for communication.</param>
    public ListCommand(McpClient mcpClient)
    {
        _mcpClient = mcpClient;
    }

    /// <summary>
    /// Executes the list command to retrieve all tasks.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteAsync()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Fetching your tasks from Todoist...");
            Console.ResetColor();

            var response = await _mcpClient.CallToolAsync("get_tasks", new { });
            
            var content = response["result"]?["content"] as JArray;
            if (content == null || content.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No tasks found or unable to retrieve tasks.");
                Console.ResetColor();
                return;
            }

            // Parse the response
            var textContent = content[0]?["text"]?.ToString();
            if (string.IsNullOrEmpty(textContent))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No tasks found.");
                Console.ResetColor();
                return;
            }

            // Display tasks
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=== Your Todoist Tasks ===");
            Console.ResetColor();
            Console.WriteLine(textContent);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error listing tasks: {ex.Message}");
            Console.ResetColor();
        }
    }
}
