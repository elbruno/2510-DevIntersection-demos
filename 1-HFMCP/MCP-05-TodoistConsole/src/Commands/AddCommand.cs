using Newtonsoft.Json.Linq;
using TodoistMcpConsole.Services;

namespace TodoistMcpConsole.Commands;

/// <summary>
/// Command to add a new task to Todoist.
/// </summary>
public class AddCommand
{
    private readonly McpClient _mcpClient;

    /// <summary>
    /// Initializes a new instance of the AddCommand.
    /// </summary>
    /// <param name="mcpClient">The MCP client to use for communication.</param>
    public AddCommand(McpClient mcpClient)
    {
        _mcpClient = mcpClient;
    }

    /// <summary>
    /// Executes the add command to create a new task.
    /// </summary>
    /// <param name="title">The title/content of the task to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Task title cannot be empty.");
            Console.ResetColor();
            return;
        }

        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Adding task: '{title}'...");
            Console.ResetColor();

            var arguments = new
            {
                content = title
            };

            var response = await _mcpClient.CallToolAsync("add_task", arguments);
            
            var content = response["result"]?["content"] as JArray;
            if (content == null || content.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Task might have been added, but unable to confirm.");
                Console.ResetColor();
                return;
            }

            var textContent = content[0]?["text"]?.ToString();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Task added successfully!");
            Console.ResetColor();
            
            if (!string.IsNullOrEmpty(textContent))
            {
                Console.WriteLine(textContent);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error adding task: {ex.Message}");
            Console.ResetColor();
        }
    }
}
