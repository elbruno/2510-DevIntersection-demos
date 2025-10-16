using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.ClientModel;
using TodoistMcpConsole.Services;

namespace TodoistMcpConsole.Commands;

/// <summary>
/// Command to start an interactive chat session with AI to manage Todoist tasks.
/// </summary>
public class ChatCommand
{
    private readonly McpClient _mcpClient;
    private readonly IConfiguration _configuration;
    private readonly List<ChatMessage> _conversationHistory;
    private List<AITool>? _tools;

    /// <summary>
    /// Initializes a new instance of the ChatCommand.
    /// </summary>
    /// <param name="mcpClient">The MCP client to use for communication.</param>
    /// <param name="configuration">The configuration to read AI settings from.</param>
    public ChatCommand(McpClient mcpClient, IConfiguration configuration)
    {
        _mcpClient = mcpClient;
        _configuration = configuration;
        _conversationHistory = new List<ChatMessage>();
    }

    /// <summary>
    /// Executes the chat command to start an interactive conversation.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteAsync()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Starting chat mode...");
            Console.WriteLine("Connecting to AI and MCP server...");
            Console.ResetColor();

            // Get available MCP tools
            _tools = await GetMcpToolsAsync();
            if (_tools == null || _tools.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: No MCP tools available.");
                Console.ResetColor();
                return;
            }

            // Create AI chat client
            var chatClient = GetChatClient();
            if (chatClient == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Could not create AI chat client. Please check your Azure OpenAI configuration.");
                Console.WriteLine("Required configuration:");
                Console.WriteLine("  - endpoint: Azure OpenAI endpoint");
                Console.WriteLine("  - apikey: Azure OpenAI API key");
                Console.WriteLine("  - deploymentName: Model deployment name (optional, defaults to gpt-4o-mini)");
                Console.ResetColor();
                return;
            }

            var chatOptions = new ChatOptions
            {
                Tools = [.. _tools]
            };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         Welcome to Todoist MCP Chat Mode! 🤖              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine("\nYou can now chat naturally with the assistant about your Todoist tasks.");
            Console.WriteLine("Examples:");
            Console.WriteLine("  • 'Show me my tasks'");
            Console.WriteLine("  • 'Add a task to buy groceries'");
            Console.WriteLine("  • 'Complete task 123456'");
            Console.WriteLine("  • 'What tasks do I have today?'");
            Console.WriteLine("\nType 'exit' or 'quit' to leave chat mode.");
            Console.WriteLine();

            // Initialize conversation with system message
            _conversationHistory.Add(new ChatMessage(
                ChatRole.System,
                "You are a helpful assistant that helps users manage their Todoist tasks. " +
                "You have access to tools to list, add, and complete tasks. " +
                "When users ask about their tasks, use the get_tasks tool. " +
                "When they want to add a task, use the add_task tool with the content parameter. " +
                "When they want to complete a task, use the complete_task tool with the task_id parameter. " +
                "Be friendly, concise, and helpful. Always confirm actions and show results clearly."
            ));

            // Main chat loop
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nYou: ");
                Console.ResetColor();
                
                var userInput = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    continue;
                }

                // Check for exit commands
                if (userInput.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    userInput.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nGoodbye! Thanks for using Todoist MCP Chat! 👋");
                    Console.ResetColor();
                    break;
                }

                try
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("Assistant: ");
                    Console.ResetColor();

                    // Get AI response with tool calling
                    var response = await chatClient.GetResponseAsync(userInput, chatOptions);

                    // Display the assistant's response
                    Console.WriteLine(response.Text);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nError: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error in chat mode: {ex.Message}");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Gets available MCP tools and converts them to AI function tools.
    /// </summary>
    private async Task<List<AITool>> GetMcpToolsAsync()
    {
        try
        {
            var toolNames = await _mcpClient.ListToolsAsync();
            var aiTools = new List<AITool>();

            // Define tool schemas based on known Todoist MCP tools
            foreach (var toolName in toolNames)
            {
                if (toolName == "get_tasks")
                {
                    aiTools.Add(AIFunctionFactory.Create(
                        async () => await ExecuteMcpToolAsync(toolName, "{}"),
                        toolName,
                        "Get all tasks from Todoist"
                    ));
                }
                else if (toolName == "add_task")
                {
                    aiTools.Add(AIFunctionFactory.Create(
                        async (string content) => 
                        {
                            var jsonArg = Newtonsoft.Json.JsonConvert.SerializeObject(new { content });
                            return await ExecuteMcpToolAsync(toolName, jsonArg);
                        },
                        toolName,
                        "Add a new task to Todoist. Parameter content: The content/title of the task (required)"
                    ));
                }
                else if (toolName == "complete_task")
                {
                    aiTools.Add(AIFunctionFactory.Create(
                        async (string task_id) => 
                        {
                            var jsonArg = Newtonsoft.Json.JsonConvert.SerializeObject(new { task_id });
                            return await ExecuteMcpToolAsync(toolName, jsonArg);
                        },
                        toolName,
                        "Mark a task as complete in Todoist. Parameter task_id: The ID of the task to complete (required)"
                    ));
                }
            }

            return aiTools;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error getting MCP tools: {ex.Message}");
            Console.ResetColor();
            return new List<AITool>();
        }
    }

    /// <summary>
    /// Executes an MCP tool with the given arguments.
    /// </summary>
    private async Task<string> ExecuteMcpToolAsync(string toolName, string arguments)
    {
        try
        {
            // Parse arguments from JSON string
            object args;
            if (string.IsNullOrWhiteSpace(arguments) || arguments == "{}")
            {
                args = new { };
            }
            else
            {
                var argsDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(arguments);
                args = argsDict != null ? (object)argsDict : new { };
            }

            // Call the MCP tool
            var response = await _mcpClient.CallToolAsync(toolName, args);
            
            // Extract text content from response
            var content = response["result"]?["content"] as JArray;
            if (content != null && content.Count > 0)
            {
                var textContent = content[0]?["text"]?.ToString();
                return textContent ?? "Tool executed successfully";
            }

            return "Tool executed successfully";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Creates an AI chat client from configuration.
    /// </summary>
    private IChatClient? GetChatClient()
    {
        try
        {
            var endpoint = _configuration["endpoint"];
            var apiKey = _configuration["apikey"];
            var deploymentName = _configuration["deploymentName"] ?? "gpt-4o-mini";

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }

            var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
                .GetChatClient(deploymentName)
                .AsIChatClient()
                .AsBuilder()
                .UseFunctionInvocation()
                .Build();

            return client;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error creating chat client: {ex.Message}");
            Console.ResetColor();
            return null;
        }
    }
}
