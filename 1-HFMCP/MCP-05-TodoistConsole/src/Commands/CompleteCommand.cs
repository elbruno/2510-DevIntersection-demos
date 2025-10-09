using Newtonsoft.Json.Linq;
using TodoistMcpConsole.Services;

namespace TodoistMcpConsole.Commands;

/// <summary>
/// Command to mark a task as complete in Todoist.
/// </summary>
public class CompleteCommand
{
    private readonly McpClient _mcpClient;

    /// <summary>
    /// Initializes a new instance of the CompleteCommand.
    /// </summary>
    /// <param name="mcpClient">The MCP client to use for communication.</param>
    public CompleteCommand(McpClient mcpClient)
    {
        _mcpClient = mcpClient;
    }

    /// <summary>
    /// Executes the complete command to mark a task as done.
    /// </summary>
    /// <param name="taskId">The ID of the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteAsync(string taskId)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Task ID cannot be empty.");
            Console.ResetColor();
            return;
        }

        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Marking task {taskId} as complete...");
            Console.ResetColor();

            var arguments = new
            {
                task_id = taskId
            };

            var response = await _mcpClient.CallToolAsync("complete_task", arguments);
            
            var content = response["result"]?["content"] as JArray;
            if (content == null || content.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Task might have been completed, but unable to confirm.");
                Console.ResetColor();
                return;
            }

            var textContent = content[0]?["text"]?.ToString();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Task marked as complete!");
            Console.ResetColor();
            
            if (!string.IsNullOrEmpty(textContent))
            {
                Console.WriteLine(textContent);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error completing task: {ex.Message}");
            Console.ResetColor();
        }
    }
}
