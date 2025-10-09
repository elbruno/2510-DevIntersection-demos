# Request/Response Flow Diagram

## Overview

This document describes the request/response flow between the Todoist MCP Console application and the Todoist MCP server.

## High-Level Flow

```
┌──────────────┐
│     User     │
└──────┬───────┘
       │
       │ Executes command
       ▼
┌──────────────────────────────────────┐
│        Console Application           │
│  ┌────────────────────────────────┐  │
│  │         Program.cs             │  │
│  │  - Parse arguments             │  │
│  │  - Load configuration          │  │
│  │  - Create McpClient            │  │
│  └────────────┬───────────────────┘  │
│               │                      │
│               │ Invokes              │
│               ▼                      │
│  ┌────────────────────────────────┐  │
│  │      Command Handler           │  │
│  │  - ListCommand                 │  │
│  │  - AddCommand                  │  │
│  │  - CompleteCommand             │  │
│  └────────────┬───────────────────┘  │
└───────────────┼──────────────────────┘
                │
                │ Calls tool
                ▼
┌─────────────────────────────────────┐
│          McpClient Service          │
│  ┌──────────────────────────────┐   │
│  │   Connection Management      │   │
│  │   - Start MCP server         │   │
│  │   - Initialize connection    │   │
│  │   - Send JSON-RPC requests   │   │
│  └──────────┬───────────────────┘   │
└─────────────┼───────────────────────┘
              │
              │ JSON-RPC over stdio
              ▼
┌──────────────────────────────────────┐
│      Todoist MCP Server (Node.js)    │
│  ┌────────────────────────────────┐  │
│  │    Request Handler             │  │
│  │    - Parse JSON-RPC            │  │
│  │    - Execute tool              │  │
│  │    - Format response           │  │
│  └────────────┬───────────────────┘  │
└────────────────┼──────────────────────┘
                 │
                 │ HTTPS
                 ▼
┌──────────────────────────────────────┐
│         Todoist REST API             │
│  - GET /tasks                        │
│  - POST /tasks                       │
│  - POST /tasks/{id}/complete         │
└──────────────────────────────────────┘
```

## Detailed Flow for "List Tasks" Command

### Step-by-Step Process

```
1. User Input
   │
   │  Command: dotnet run -- list
   │
   ▼
2. Program.cs
   │
   │  • Parse command line arguments
   │  • Load configuration (API token, timeouts)
   │  • Create McpClient instance
   │
   ▼
3. McpClient.ConnectAsync()
   │
   │  • Start MCP server: npx -y @doist/todoist-ai-mcp-server
   │  • Set TODOIST_API_TOKEN environment variable
   │  • Capture stdin/stdout/stderr streams
   │
   ▼
4. McpClient.InitializeAsync()
   │
   │  Request:
   │  {
   │    "jsonrpc": "2.0",
   │    "id": 1,
   │    "method": "initialize",
   │    "params": {
   │      "protocolVersion": "2024-11-05",
   │      "capabilities": {},
   │      "clientInfo": {
   │        "name": "TodoistMcpConsole",
   │        "version": "1.0.0"
   │      }
   │    }
   │  }
   │
   │  Response:
   │  {
   │    "jsonrpc": "2.0",
   │    "id": 1,
   │    "result": {
   │      "protocolVersion": "2024-11-05",
   │      "capabilities": { ... },
   │      "serverInfo": { ... }
   │    }
   │  }
   │
   │  Notification:
   │  {
   │    "jsonrpc": "2.0",
   │    "method": "notifications/initialized"
   │  }
   │
   ▼
5. ListCommand.ExecuteAsync()
   │
   │  • Call McpClient.CallToolAsync("get_tasks", {})
   │
   ▼
6. McpClient.CallToolAsync()
   │
   │  Request:
   │  {
   │    "jsonrpc": "2.0",
   │    "id": 2,
   │    "method": "tools/call",
   │    "params": {
   │      "name": "get_tasks",
   │      "arguments": {}
   │    }
   │  }
   │
   │  (If timeout or error, retry with exponential backoff)
   │
   ▼
7. MCP Server
   │
   │  • Parse JSON-RPC request
   │  • Extract tool name and arguments
   │  • Call Todoist REST API: GET https://api.todoist.com/rest/v2/tasks
   │  • Format response as MCP content
   │
   │  Response:
   │  {
   │    "jsonrpc": "2.0",
   │    "id": 2,
   │    "result": {
   │      "content": [
   │        {
   │          "type": "text",
   │          "text": "1. [ID: 123] Buy groceries\n2. [ID: 456] Call dentist"
   │        }
   │      ]
   │    }
   │  }
   │
   ▼
8. ListCommand
   │
   │  • Parse response
   │  • Extract task list from content
   │  • Display with colors:
   │    - Cyan for "Fetching tasks..."
   │    - Green for "=== Your Todoist Tasks ==="
   │    - White for task details
   │
   ▼
9. User sees output
   │
   │  === Your Todoist Tasks ===
   │  1. [ID: 123] Buy groceries
   │  2. [ID: 456] Call dentist
   │
   └─► Program exits
```

## Detailed Flow for "Add Task" Command

```
1. User Input: dotnet run -- add "Buy milk"
   │
   ▼
2. Program.cs → Parse arguments → title = "Buy milk"
   │
   ▼
3. McpClient.ConnectAsync() → Initialize MCP connection
   │
   ▼
4. AddCommand.ExecuteAsync("Buy milk")
   │
   ▼
5. McpClient.CallToolAsync("add_task", { content: "Buy milk" })
   │
   │  Request:
   │  {
   │    "jsonrpc": "2.0",
   │    "id": 2,
   │    "method": "tools/call",
   │    "params": {
   │      "name": "add_task",
   │      "arguments": {
   │        "content": "Buy milk"
   │      }
   │    }
   │  }
   │
   ▼
6. MCP Server → Todoist API: POST /tasks { "content": "Buy milk" }
   │
   │  Response:
   │  {
   │    "jsonrpc": "2.0",
   │    "id": 2,
   │    "result": {
   │      "content": [
   │        {
   │          "type": "text",
   │          "text": "Task created with ID: 789"
   │        }
   │      ]
   │    }
   │  }
   │
   ▼
7. AddCommand → Display success message
   │
   │  ✓ Task added successfully!
   │  Task created with ID: 789
   │
   └─► Program exits
```

## Detailed Flow for "Complete Task" Command

```
1. User Input: dotnet run -- complete 123
   │
   ▼
2. Program.cs → Parse arguments → id = "123"
   │
   ▼
3. McpClient.ConnectAsync() → Initialize MCP connection
   │
   ▼
4. CompleteCommand.ExecuteAsync("123")
   │
   ▼
5. McpClient.CallToolAsync("complete_task", { task_id: "123" })
   │
   │  Request:
   │  {
   │    "jsonrpc": "2.0",
   │    "id": 2,
   │    "method": "tools/call",
   │    "params": {
   │      "name": "complete_task",
   │      "arguments": {
   │        "task_id": "123"
   │      }
   │    }
   │  }
   │
   ▼
6. MCP Server → Todoist API: POST /tasks/123/complete
   │
   │  Response:
   │  {
   │    "jsonrpc": "2.0",
   │    "id": 2,
   │    "result": {
   │      "content": [
   │        {
   │          "type": "text",
   │          "text": "Task 123 has been completed."
   │        }
   │      ]
   │    }
   │  }
   │
   ▼
7. CompleteCommand → Display success message
   │
   │  ✓ Task marked as complete!
   │  Task 123 has been completed.
   │
   └─► Program exits
```

## Error Handling Flow

```
Request initiated
   │
   ▼
Try to send request
   │
   ├──► Success → Return response
   │
   └──► Failure (timeout/error)
          │
          ▼
       Retry attempt 1
       (wait 2 seconds)
          │
          ├──► Success → Return response
          │
          └──► Failure
                 │
                 ▼
              Retry attempt 2
              (wait 4 seconds)
                 │
                 ├──► Success → Return response
                 │
                 └──► Failure
                        │
                        ▼
                     Retry attempt 3
                     (wait 8 seconds)
                        │
                        ├──► Success → Return response
                        │
                        └──► Failure
                               │
                               ▼
                          Throw exception
                          Display error message
```

## Configuration Flow

```
Application starts
   │
   ▼
Load configuration sources (in order of precedence):
   │
   ├─► 1. Command line arguments
   ├─► 2. User Secrets (.NET User Secrets)
   ├─► 3. Environment Variables
   └─► 4. appsettings.json
   │
   ▼
Get TODOIST_API_TOKEN
   │
   ├──► Found → Continue
   │
   └──► Not found → Display error and exit
   │
   ▼
Get MCP server configuration:
   - ServerPath (default: "npx")
   - ServerArgs (default: "-y @doist/todoist-ai-mcp-server")
   - RequestTimeout (default: 60 seconds)
   - MaxRetries (default: 3)
   │
   ▼
Start MCP client with configuration
```

## JSON-RPC Message Format

### Request Format
```json
{
  "jsonrpc": "2.0",
  "id": <numeric_id>,
  "method": "<method_name>",
  "params": {
    // method-specific parameters
  }
}
```

### Response Format (Success)
```json
{
  "jsonrpc": "2.0",
  "id": <numeric_id>,
  "result": {
    // method-specific result
  }
}
```

### Response Format (Error)
```json
{
  "jsonrpc": "2.0",
  "id": <numeric_id>,
  "error": {
    "code": <error_code>,
    "message": "<error_message>",
    "data": {
      // optional additional error data
    }
  }
}
```

### Notification Format (No Response Expected)
```json
{
  "jsonrpc": "2.0",
  "method": "<notification_name>",
  "params": {
    // notification-specific parameters
  }
}
```
