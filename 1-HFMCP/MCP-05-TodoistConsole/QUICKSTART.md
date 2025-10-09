# Quick Start Guide - Todoist MCP Console

Get up and running with the Todoist MCP Console in 5 minutes!

## Prerequisites

✅ [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)  
✅ [Node.js and npm](https://nodejs.org/)  
✅ [Todoist API Token](https://todoist.com/prefs/integrations)

## Installation

```bash
# Clone the repository
git clone <repository-url>
cd 1-HFMCP/MCP-05-TodoistConsole/src

# Set your API token (choose one method)
dotnet user-secrets set "TODOIST_API_TOKEN" "your-api-token-here"
# OR
export TODOIST_API_TOKEN="your-api-token-here"

# Build the project
dotnet build
```

## Usage

### List all tasks
```bash
dotnet run -- list
```

### Add a new task
```bash
dotnet run -- add "Buy groceries"
```

### Complete a task
```bash
dotnet run -- complete 1234567890
```

### See available tools
```bash
dotnet run -- tools
```

### Get help
```bash
dotnet run -- --help
```

## Example Session

```bash
# Add a task
$ dotnet run -- add "Prepare presentation for DevIntersection"
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Adding task: 'Prepare presentation for DevIntersection'...
✓ Task added successfully!
Task created with ID: 7234567890

# List tasks to see it
$ dotnet run -- list
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Fetching your tasks from Todoist...

=== Your Todoist Tasks ===
Found 1 task:
1. [ID: 7234567890] Prepare presentation for DevIntersection

# Complete the task
$ dotnet run -- complete 7234567890
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Marking task 7234567890 as complete...
✓ Task marked as complete!
```

## Troubleshooting

### "TODOIST_API_TOKEN not found"
Set your API token using one of these methods:
```bash
dotnet user-secrets set "TODOIST_API_TOKEN" "your-token"
# OR
export TODOIST_API_TOKEN="your-token"
```

### "Failed to start MCP server"
Make sure Node.js is installed:
```bash
node --version  # Should show v18+ or higher
npm --version
```

### Network timeout
Increase timeout in `appsettings.json`:
```json
{
  "McpServer": {
    "RequestTimeout": 120,
    "MaxRetries": 5
  }
}
```

## Next Steps

- Read the full [README.md](README.md) for detailed documentation
- Check [docs/flow-diagram.md](docs/flow-diagram.md) to understand the architecture
- Review [sample_data/sample-transcripts.md](sample_data/sample-transcripts.md) for more examples

## Support

For issues or questions:
1. Check the [README.md](README.md) troubleshooting section
2. Review the [GitHub Issues](https://github.com/elbruno/2510-DevIntersection-demos/issues)
3. Consult the [Todoist API documentation](https://developer.todoist.com/)

---

**Happy task managing! 🎯**
