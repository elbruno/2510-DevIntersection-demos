# Todoist MCP Console Application

A C# .NET 9 console application that connects to a Todoist MCP (Model Context Protocol) server to manage your Todoist tasks directly from the command line.

## Features

- ✅ **List Tasks**: View all your Todoist tasks
- ✅ **Add Tasks**: Create new tasks with a title
- ✅ **Complete Tasks**: Mark tasks as complete by ID
- ✅ **MCP Integration**: Uses JSON-RPC over stdio for communication
- ✅ **Robust Error Handling**: Automatic retries with exponential backoff
- ✅ **Colorful Console**: User-friendly output with color-coded messages
- ✅ **Flexible Configuration**: Support for User Secrets, environment variables, and appsettings.json

## Prerequisites

### Required Software

1. **.NET 9 SDK**: Download and install from [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)

2. **Node.js and npm**: Required to run the Todoist MCP server
   - Download from [https://nodejs.org/](https://nodejs.org/)
   - Verify installation: `node --version` and `npm --version`

3. **Todoist API Token**: Get your API token from [https://todoist.com/prefs/integrations](https://todoist.com/prefs/integrations)

### MCP Server Setup

The application uses the official Todoist MCP server from [@doist/todoist-ai-mcp-server](https://github.com/doist/todoist-ai). The server is automatically started by the application using `npx`.

## Installation

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd 1-HFMCP/MCP-05-TodoistConsole
   ```

2. **Build the project**:
   ```bash
   cd src
   dotnet build
   ```

3. **Run tests** (optional):
   ```bash
   cd ../tests
   dotnet test
   ```

## Configuration

You can configure your Todoist API token using any of these methods (in order of precedence):

### Method 1: User Secrets (Recommended for Development)

```bash
cd src
dotnet user-secrets set "TODOIST_API_TOKEN" "your-api-token-here"
```

### Method 2: Environment Variables

**Windows (PowerShell)**:
```powershell
$env:TODOIST_API_TOKEN="your-api-token-here"
```

**Linux/macOS**:
```bash
export TODOIST_API_TOKEN="your-api-token-here"
```

### Method 3: appsettings.json

Edit `src/appsettings.json` and add:
```json
{
  "TodoistApiToken": "your-api-token-here"
}
```

⚠️ **Note**: Never commit your API token to version control!

## Usage

### Run the Application

From the `src` directory:

```bash
dotnet run
```

### Available Commands

#### List all tasks
```bash
dotnet run -- list
```

**Example output**:
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Fetching your tasks from Todoist...

=== Your Todoist Tasks ===
1. [ID: 123456789] Buy groceries (Project: Personal)
2. [ID: 987654321] Finish report (Project: Work)
3. [ID: 456789123] Call dentist (Project: Personal)
```

#### Add a new task
```bash
dotnet run -- add "Buy milk and eggs"
```

**Example output**:
```
Adding task: 'Buy milk and eggs'...
✓ Task added successfully!
Task created with ID: 234567890
```

#### Complete a task
```bash
dotnet run -- complete 123456789
```

**Example output**:
```
Marking task 123456789 as complete...
✓ Task marked as complete!
Task 123456789 has been completed.
```

#### List available MCP tools
```bash
dotnet run -- tools
```

**Example output**:
```
Connecting to Todoist MCP server...

=== Available MCP Tools ===
  • get_tasks
  • add_task
  • complete_task
  • update_task
```

### Get Help

```bash
dotnet run -- --help
```

## Architecture

### Project Structure

```
MCP-05-TodoistConsole/
├── src/
│   ├── Commands/
│   │   ├── ListCommand.cs      # Handles listing tasks
│   │   ├── AddCommand.cs       # Handles adding tasks
│   │   └── CompleteCommand.cs  # Handles completing tasks
│   ├── Services/
│   │   └── McpClient.cs        # MCP client with JSON-RPC over stdio
│   ├── Program.cs              # Main entry point
│   ├── appsettings.json        # Configuration file
│   └── TodoistMcpConsole.csproj
├── tests/
│   ├── McpClientTests.cs       # Unit tests
│   └── TodoistMcpConsole.Tests.csproj
├── docs/
│   ├── flow-diagram.md         # Request/response flow
│   └── sequence-diagram.md     # Tool invocation sequence
├── sample_data/
│   └── sample-transcripts.md   # Example command outputs
└── README.md
```

### Architecture Diagram

```
┌─────────────────┐
│   Console App   │
│   (Program.cs)  │
└────────┬────────┘
         │
         │ Uses
         ▼
┌─────────────────┐      JSON-RPC      ┌──────────────────┐
│   McpClient     │◄────over stdio────►│  Todoist MCP     │
│   (Services)    │                    │     Server       │
└────────┬────────┘                    └────────┬─────────┘
         │                                      │
         │ Invoked by                           │ Calls
         ▼                                      ▼
┌─────────────────┐                    ┌──────────────────┐
│    Commands     │                    │   Todoist API    │
│ (List/Add/      │                    │  (REST API)      │
│  Complete)      │                    └──────────────────┘
└─────────────────┘
```

### Communication Flow

1. **Console App** → Parses command line arguments
2. **Program.cs** → Initializes configuration and creates McpClient
3. **McpClient** → Starts MCP server process (via npx)
4. **McpClient** → Sends JSON-RPC `initialize` request
5. **MCP Server** → Responds with available capabilities
6. **McpClient** → Sends `initialized` notification
7. **Command** → Calls McpClient with tool name and arguments
8. **McpClient** → Sends JSON-RPC `tools/call` request
9. **MCP Server** → Invokes Todoist API
10. **MCP Server** → Returns response
11. **Command** → Displays formatted output

See [docs/flow-diagram.md](docs/flow-diagram.md) and [docs/sequence-diagram.md](docs/sequence-diagram.md) for detailed diagrams.

## Error Handling

The application includes comprehensive error handling:

- **Retry Logic**: Automatic retries with exponential backoff (configurable)
- **Timeout Handling**: Configurable timeouts for requests
- **Validation**: Input validation for all commands
- **User-Friendly Messages**: Color-coded error messages with helpful information

### Configuration Options

Edit `appsettings.json` to customize behavior:

```json
{
  "McpServer": {
    "TodoistServerPath": "npx",
    "TodoistServerArgs": "-y @doist/todoist-ai-mcp-server",
    "ConnectionTimeout": 30,
    "RequestTimeout": 60,
    "MaxRetries": 3
  }
}
```

## Development

### Building

```bash
cd src
dotnet build
```

### Testing

```bash
cd tests
dotnet test
```

### Creating a Release Build

```bash
cd src
dotnet publish -c Release -o ../publish
```

The compiled application will be in the `publish` directory.

## Troubleshooting

### "TODOIST_API_TOKEN not found!"

**Solution**: Configure your API token using one of the methods described in the [Configuration](#configuration) section.

### "Failed to start MCP server process"

**Solution**: 
1. Verify Node.js and npm are installed: `node --version` and `npm --version`
2. Try manually installing the MCP server: `npm install -g @doist/todoist-ai-mcp-server`
3. Check that the server path in `appsettings.json` is correct

### "Failed to communicate with MCP server"

**Solution**:
1. Check your internet connection
2. Verify your Todoist API token is valid
3. Increase the timeout values in `appsettings.json`

### Tests failing

**Solution**:
1. Ensure .NET 9 SDK is installed
2. Restore packages: `dotnet restore`
3. Clean and rebuild: `dotnet clean && dotnet build`

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch
3. Write tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## License

This project is part of the DevIntersection demos repository.

## Additional Resources

- [Todoist API Documentation](https://developer.todoist.com/)
- [MCP (Model Context Protocol) Specification](https://modelcontextprotocol.io/)
- [.NET 9 Documentation](https://docs.microsoft.com/dotnet/)
- [Todoist MCP Server Repository](https://github.com/doist/todoist-ai)

## Sample Data

See [sample_data/sample-transcripts.md](sample_data/sample-transcripts.md) for example command outputs and transcripts.
