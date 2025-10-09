# Chat Mode - Overview & Architecture

## Introduction

The Todoist MCP Console now includes an interactive chat mode that allows users to manage their Todoist tasks through natural language conversations with an AI assistant powered by Azure OpenAI.

## Key Features

### 🗣️ Natural Language Interface
Users can interact with their tasks using everyday language instead of memorizing command syntax:

**Traditional Way:**
```bash
dotnet run -- add "Buy groceries"
```

**Chat Mode:**
```
You: I need to buy groceries
Assistant: I'll add that task for you... ✓ Task added!
```

### 🤖 AI-Powered Understanding
The AI assistant understands intent and context:
- "Show my tasks" → Calls `get_tasks` tool
- "Add a reminder to..." → Calls `add_task` tool  
- "I finished task 123" → Calls `complete_task` tool

### 💬 Conversational Flow
Maintains conversation context for multi-turn interactions:
```
You: What tasks do I have?
Assistant: You have 3 tasks...

You: Complete the first one
Assistant: [remembers context] ✓ Completed!
```

### 🎯 Tool Transparency
Users can see when MCP tools are being invoked:
```
You: Show my tasks
  [Calling tool: get_tasks]
Assistant: Here are your tasks...
```

## Architecture

### Component Flow

```
┌─────────────────────────────────────────────────────────┐
│                    User Input                           │
│           "Show me my tasks for today"                  │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│                 ChatCommand.cs                          │
│  • Manages conversation loop                            │
│  • Maintains chat history                               │
│  • Handles user input/output                            │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│            Azure OpenAI (IChatClient)                   │
│  • Natural language understanding                       │
│  • Intent detection                                     │
│  • Function calling decision                            │
│  • Response generation                                  │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼ (Function Call)
┌─────────────────────────────────────────────────────────┐
│              AITool (get_tasks, add_task, etc.)         │
│  • Wraps MCP tool invocation                            │
│  • Handles parameter extraction                         │
│  • Formats tool arguments                               │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│                   McpClient.cs                          │
│  • JSON-RPC communication                               │
│  • Stdio process management                             │
│  • Calls MCP tool                                       │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│              Todoist MCP Server (Node.js)               │
│  • Executes tool (get_tasks, add_task, etc.)            │
│  • Calls Todoist REST API                               │
│  • Returns structured response                          │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│                 Todoist REST API                        │
│  • Manages tasks in Todoist                             │
│  • Returns task data                                    │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼ (Response flows back up)
┌─────────────────────────────────────────────────────────┐
│                    User Output                          │
│  Assistant: Here are your tasks:                        │
│  1. [ID: 123] Buy groceries                             │
│  2. [ID: 456] Finish report                             │
└─────────────────────────────────────────────────────────┘
```

## Implementation Details

### ChatCommand Class Structure

```csharp
public class ChatCommand
{
    private readonly McpClient _mcpClient;
    private readonly IConfiguration _configuration;
    private readonly List<ChatMessage> _conversationHistory;
    private List<AITool>? _tools;

    // Main execution loop
    public async Task ExecuteAsync()
    {
        // 1. Initialize AI client and MCP tools
        // 2. Display welcome message
        // 3. Enter chat loop
        // 4. Process user input
        // 5. Get AI response
        // 6. Display results
    }

    // Convert MCP tools to AI function tools
    private async Task<List<AITool>> GetMcpToolsAsync()
    {
        // Registers get_tasks, add_task, complete_task
    }

    // Execute MCP tool with arguments
    private async Task<string> ExecuteMcpToolAsync(...)
    {
        // Calls McpClient.CallToolAsync()
    }

    // Create Azure OpenAI chat client
    private IChatClient? GetChatClient()
    {
        // Configures Azure OpenAI with function calling
    }
}
```

### Tool Registration

Each MCP tool is registered as an AI function:

```csharp
// get_tasks - no parameters
AIFunctionFactory.Create(
    async () => await ExecuteMcpToolAsync("get_tasks", "{}"),
    "get_tasks",
    "Get all tasks from Todoist"
)

// add_task - requires content parameter
AIFunctionFactory.Create(
    async (string content) => {
        var jsonArg = JsonConvert.SerializeObject(new { content });
        return await ExecuteMcpToolAsync("add_task", jsonArg);
    },
    "add_task",
    "Add a new task to Todoist. Parameter content: The content/title (required)"
)

// complete_task - requires task_id parameter
AIFunctionFactory.Create(
    async (string task_id) => {
        var jsonArg = JsonConvert.SerializeObject(new { task_id });
        return await ExecuteMcpToolAsync("complete_task", jsonArg);
    },
    "complete_task",
    "Mark a task as complete. Parameter task_id: The task ID (required)"
)
```

### AI Decision Process

1. **User Input**: "Add a task to buy milk"
2. **AI Analysis**: 
   - Intent: Adding a task
   - Required tool: add_task
   - Parameter: content = "buy milk"
3. **Function Call**: `add_task(content: "buy milk")`
4. **Tool Execution**: MCP tool called with parameters
5. **Response**: Task created with ID 123456
6. **AI Response**: "✓ Task added! 'Buy milk' has been created with ID 123456"

## Configuration

### Required Settings (for chat mode)

```json
{
  "endpoint": "https://your-endpoint.openai.azure.com/",
  "apikey": "your-api-key",
  "deploymentName": "gpt-4o-mini"  // Optional, defaults to gpt-4o-mini
}
```

### Configuration Methods

1. **User Secrets** (Recommended):
   ```bash
   dotnet user-secrets set "endpoint" "https://..."
   dotnet user-secrets set "apikey" "..."
   ```

2. **Environment Variables**:
   ```bash
   export endpoint="https://..."
   export apikey="..."
   ```

3. **appsettings.json**:
   ```json
   {
     "endpoint": "https://...",
     "apikey": "..."
   }
   ```

## User Experience Flow

### Starting Chat

```
$ dotnet run -- chat

╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Starting chat mode...
Connecting to AI and MCP server...

╔════════════════════════════════════════════════════════════╗
║         Welcome to Todoist MCP Chat Mode! 🤖              ║
╚════════════════════════════════════════════════════════════╝

You can now chat naturally with the assistant about your Todoist tasks.
Examples:
  • 'Show me my tasks'
  • 'Add a task to buy groceries'
  • 'Complete task 123456'
  • 'What tasks do I have today?'

Type 'exit' or 'quit' to leave chat mode.
```

### Interactive Session

```
You: Show my tasks
  [Calling tool: get_tasks]
Assistant: Here are your current tasks:
1. [ID: 123456] Buy groceries
2. [ID: 789012] Finish report

You: Add prepare demo
  [Calling tool: add_task]
Assistant: ✓ Task added: "Prepare demo"

You: Complete 123456
  [Calling tool: complete_task]
Assistant: ✓ Task completed!

You: exit
Goodbye! Thanks for using Todoist MCP Chat! 👋
```

## Error Handling

### Missing Configuration
```
Error: Could not create AI chat client. Please check your Azure OpenAI configuration.
Required configuration:
  - endpoint: Azure OpenAI endpoint
  - apikey: Azure OpenAI API key
  - deploymentName: Model deployment name (optional, defaults to gpt-4o-mini)
```

### Invalid Task ID
```
You: Complete task 999999
  [Calling tool: complete_task]
Assistant: I encountered an issue - that task ID doesn't exist. 
Would you like me to show you your current tasks?
```

### Network Issues
```
Error: Failed to communicate with MCP server after 3 retries.
Troubleshooting tips:
  • Check your internet connection
  • Verify your Todoist API token is valid
```

## Performance Characteristics

### Response Times

| Operation | Typical Time | With AI Processing |
|-----------|--------------|-------------------|
| User input | < 1ms | N/A |
| AI analysis | N/A | 500-1500ms |
| Tool call | 200-1000ms | +200-1000ms |
| Response display | < 10ms | N/A |
| **Total** | N/A | **700-2500ms** |

### API Usage

- **Tokens per request**: ~50-200 tokens (input)
- **Tokens per response**: ~100-500 tokens (output)
- **Conversation context**: Grows with each message
- **Cost**: Approximately $0.001-0.005 per interaction (gpt-4o-mini)

## Comparison: Direct vs Chat Mode

### Direct Commands

**Pros:**
- ✅ Fast (no AI processing)
- ✅ Predictable
- ✅ Scriptable
- ✅ No API costs
- ✅ No additional dependencies

**Cons:**
- ❌ Requires exact syntax
- ❌ One command at a time
- ❌ No conversation context
- ❌ Less intuitive for new users

### Chat Mode

**Pros:**
- ✅ Natural language
- ✅ Conversational
- ✅ Context-aware
- ✅ Intuitive
- ✅ Flexible queries
- ✅ Error recovery assistance

**Cons:**
- ❌ Requires Azure OpenAI
- ❌ API costs (token usage)
- ❌ Slightly slower
- ❌ Network dependency
- ❌ Less predictable output

## Best Practices

### When to Use Chat Mode

✅ **Good for:**
- Interactive task management sessions
- Exploring your tasks ("What do I have today?")
- Complex queries ("What's my highest priority?")
- Learning the system
- Natural conversation flow

❌ **Not ideal for:**
- Scripting/automation
- Batch operations
- CI/CD pipelines
- When API costs are a concern
- Offline scenarios

### Tips for Best Experience

1. **Be Natural**: Talk as you would to a colleague
   - "Show my tasks" ✅
   - "list" ✅ (works too, but less natural)

2. **Use Task IDs**: Be specific when referencing tasks
   - "Complete task 123456" ✅
   - "Complete the first one" ⚠️ (relies on AI memory)

3. **Ask for Help**: The AI can clarify what you need
   - "I'm not sure what to do" ✅

4. **Context is King**: Reference previous messages
   - "Add that to my tasks" ✅
   - "Also add..." ✅

5. **Exit Gracefully**: Use 'exit' or 'quit' commands
   - "exit" ✅
   - "bye" ✅
   - Ctrl+C ⚠️ (works but less clean)

## Technical Requirements

### Dependencies

```xml
<PackageReference Include="Azure.AI.OpenAI" Version="2.5.0-beta.1" />
<PackageReference Include="Microsoft.Extensions.AI" Version="9.9.1" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.9.1-preview.1.25474.6" />
```

### Minimum Requirements

- .NET 9.0 SDK
- Azure OpenAI account (for chat mode only)
- Internet connection
- Node.js (for MCP server)
- Todoist account with API token

## Future Enhancements

### Potential Improvements

1. **Multi-Model Support**
   - Add GitHub Models support
   - Add local LLM support (via Ollama)
   - Allow model selection

2. **Enhanced Features**
   - Task filtering in chat ("Show high priority")
   - Date parsing ("Due tomorrow")
   - Project management
   - Label operations

3. **Conversation Management**
   - Save/load conversation history
   - Export transcripts
   - Clear history command

4. **Offline Mode**
   - Cache common responses
   - Fallback to direct commands

5. **Performance**
   - Streaming responses
   - Parallel tool calls
   - Response caching

## Conclusion

The chat mode provides a natural, conversational interface to the Todoist MCP Console while maintaining full backward compatibility with direct commands. Users can choose the interaction style that best fits their needs and workflow.

For quick, scriptable operations, use direct commands. For exploratory, conversational task management, use chat mode. Both are first-class experiences in the same application!
