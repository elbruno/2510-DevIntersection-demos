# Tool Invocation Sequence Diagram

## Overview

This document provides detailed sequence diagrams showing the interactions between components during tool invocations.

## Sequence Diagram: List Tasks

```
User            Program.cs      McpClient       MCP Server      Todoist API
  │                 │               │                │               │
  │  run -- list    │               │                │               │
  ├────────────────>│               │                │               │
  │                 │               │                │               │
  │                 │ Load config   │                │               │
  │                 │ Get API token │                │               │
  │                 │               │                │               │
  │                 │ ConnectAsync()│                │               │
  │                 ├──────────────>│                │               │
  │                 │               │ Start process  │               │
  │                 │               │ npx @doist/... │               │
  │                 │               ├───────────────>│               │
  │                 │               │                │               │
  │                 │               │ initialize     │               │
  │                 │               ├───────────────>│               │
  │                 │               │                │               │
  │                 │               │ result         │               │
  │                 │               │<───────────────┤               │
  │                 │               │                │               │
  │                 │               │ initialized    │               │
  │                 │               ├───────────────>│               │
  │                 │               │                │               │
  │                 │ Client ready  │                │               │
  │                 │<──────────────┤                │               │
  │                 │               │                │               │
  │                 │ ListCommand() │                │               │
  │                 │ ExecuteAsync()│                │               │
  │                 │               │                │               │
  │                 │               │ CallToolAsync()│               │
  │                 │               │ "get_tasks"    │               │
  │                 │               │                │               │
  │                 │               │ tools/call     │               │
  │                 │               ├───────────────>│               │
  │                 │               │                │               │
  │                 │               │                │ GET /tasks    │
  │                 │               │                ├──────────────>│
  │                 │               │                │               │
  │                 │               │                │ Task list     │
  │                 │               │                │<──────────────┤
  │                 │               │                │               │
  │                 │               │ result         │               │
  │                 │               │<───────────────┤               │
  │                 │               │                │               │
  │                 │ Display tasks │                │               │
  │                 │<──────────────────────────────────             │
  │                 │               │                │               │
  │  Task list      │               │                │               │
  │<────────────────┤               │                │               │
  │                 │               │                │               │
  │                 │ Dispose()     │                │               │
  │                 ├──────────────>│                │               │
  │                 │               │ Kill process   │               │
  │                 │               ├───────────────>│               │
  │                 │               │                │               │
```

## Sequence Diagram: Add Task

```
User            Program.cs      McpClient       MCP Server      Todoist API
  │                 │               │                │               │
  │  run -- add     │               │                │               │
  │  "Buy milk"     │               │                │               │
  ├────────────────>│               │                │               │
  │                 │               │                │               │
  │                 │ Parse args    │                │               │
  │                 │ title="Buy    │                │               │
  │                 │ milk"         │                │               │
  │                 │               │                │               │
  │                 │ ConnectAsync()│                │               │
  │                 ├──────────────>│                │               │
  │                 │               │ Start & init   │               │
  │                 │               │ (same as above)│               │
  │                 │               │                │               │
  │                 │ Client ready  │                │               │
  │                 │<──────────────┤                │               │
  │                 │               │                │               │
  │                 │ AddCommand()  │                │               │
  │                 │ ExecuteAsync  │                │               │
  │                 │ ("Buy milk")  │                │               │
  │                 │               │                │               │
  │                 │               │ CallToolAsync()│               │
  │                 │               │ "add_task"     │               │
  │                 │               │ {content:"Buy  │               │
  │                 │               │  milk"}        │               │
  │                 │               │                │               │
  │                 │               │ tools/call     │               │
  │                 │               ├───────────────>│               │
  │                 │               │                │               │
  │                 │               │                │ POST /tasks   │
  │                 │               │                │ {content:     │
  │                 │               │                │  "Buy milk"}  │
  │                 │               │                ├──────────────>│
  │                 │               │                │               │
  │                 │               │                │ Task created  │
  │                 │               │                │ {id: 789,...} │
  │                 │               │                │<──────────────┤
  │                 │               │                │               │
  │                 │               │ result         │               │
  │                 │               │ {content:[...]}│               │
  │                 │               │<───────────────┤               │
  │                 │               │                │               │
  │                 │ Display       │                │               │
  │                 │ "Task added"  │                │               │
  │                 │<──────────────────────────────────             │
  │                 │               │                │               │
  │  ✓ Task added   │               │                │               │
  │  ID: 789        │               │                │               │
  │<────────────────┤               │                │               │
  │                 │               │                │               │
```

## Sequence Diagram: Complete Task

```
User            Program.cs      McpClient       MCP Server      Todoist API
  │                 │               │                │               │
  │  run --         │               │                │               │
  │  complete 789   │               │                │               │
  ├────────────────>│               │                │               │
  │                 │               │                │               │
  │                 │ Parse args    │                │               │
  │                 │ id="789"      │                │               │
  │                 │               │                │               │
  │                 │ ConnectAsync()│                │               │
  │                 ├──────────────>│                │               │
  │                 │               │ Start & init   │               │
  │                 │               │ (same as above)│               │
  │                 │               │                │               │
  │                 │ Client ready  │                │               │
  │                 │<──────────────┤                │               │
  │                 │               │                │               │
  │                 │CompleteCommand│                │               │
  │                 │ExecuteAsync   │                │               │
  │                 │("789")        │                │               │
  │                 │               │                │               │
  │                 │               │ CallToolAsync()│               │
  │                 │               │"complete_task" │               │
  │                 │               │{task_id:"789"} │               │
  │                 │               │                │               │
  │                 │               │ tools/call     │               │
  │                 │               ├───────────────>│               │
  │                 │               │                │               │
  │                 │               │                │POST /tasks/789│
  │                 │               │                │/complete      │
  │                 │               │                ├──────────────>│
  │                 │               │                │               │
  │                 │               │                │ Success       │
  │                 │               │                │<──────────────┤
  │                 │               │                │               │
  │                 │               │ result         │               │
  │                 │               │ {content:[...]}│               │
  │                 │               │<───────────────┤               │
  │                 │               │                │               │
  │                 │ Display       │                │               │
  │                 │ "Task complete│                │               │
  │                 │<──────────────────────────────────             │
  │                 │               │                │               │
  │  ✓ Task         │               │                │               │
  │  complete       │               │                │               │
  │<────────────────┤               │                │               │
  │                 │               │                │               │
```

## Sequence Diagram: Error Handling with Retry

```
User            Program.cs      McpClient       MCP Server      Todoist API
  │                 │               │                │               │
  │  run -- list    │               │                │               │
  ├────────────────>│               │                │               │
  │                 │               │                │               │
  │                 │ ConnectAsync()│                │               │
  │                 ├──────────────>│                │               │
  │                 │               │                │               │
  │                 │               │ tools/call     │               │
  │                 │               ├───────────────>│               │
  │                 │               │                │               │
  │                 │               │  ╳ Timeout     │               │
  │                 │               │                │               │
  │                 │               │ Wait 2s        │               │
  │                 │               │ (Retry 1/3)    │               │
  │                 │               │                │               │
  │                 │               │ tools/call     │               │
  │                 │               ├───────────────>│               │
  │                 │               │                │               │
  │                 │               │  ╳ Timeout     │               │
  │                 │               │                │               │
  │                 │               │ Wait 4s        │               │
  │                 │               │ (Retry 2/3)    │               │
  │                 │               │                │               │
  │                 │               │ tools/call     │               │
  │                 │               ├───────────────>│               │
  │                 │               │                │               │
  │                 │               │                │ GET /tasks    │
  │                 │               │                ├──────────────>│
  │                 │               │                │               │
  │                 │               │                │ Task list     │
  │                 │               │                │<──────────────┤
  │                 │               │                │               │
  │                 │               │ result ✓       │               │
  │                 │               │<───────────────┤               │
  │                 │               │                │               │
  │                 │ Display tasks │                │               │
  │                 │<──────────────────────────────────             │
  │                 │               │                │               │
  │  Task list      │               │                │               │
  │<────────────────┤               │                │               │
  │                 │               │                │               │
```

## Sequence Diagram: Maximum Retries Exceeded

```
User            Program.cs      McpClient       MCP Server      Todoist API
  │                 │               │                │               │
  │  run -- list    │               │                │               │
  ├────────────────>│               │                │               │
  │                 │               │                │               │
  │                 │ ConnectAsync()│                │               │
  │                 ├──────────────>│                │               │
  │                 │               │                │               │
  │                 │               │ tools/call     │               │
  │                 │               ├───────────────>│               │
  │                 │               │  ╳ Timeout     │               │
  │                 │               │                │               │
  │                 │               │ Wait 2s        │               │
  │                 │               │ (Retry 1/3)    │               │
  │                 │               │                │               │
  │                 │               │ tools/call     │               │
  │                 │               ├───────────────>│               │
  │                 │               │  ╳ Timeout     │               │
  │                 │               │                │               │
  │                 │               │ Wait 4s        │               │
  │                 │               │ (Retry 2/3)    │               │
  │                 │               │                │               │
  │                 │               │ tools/call     │               │
  │                 │               ├───────────────>│               │
  │                 │               │  ╳ Timeout     │               │
  │                 │               │                │               │
  │                 │               │ Wait 8s        │               │
  │                 │               │ (Retry 3/3)    │               │
  │                 │               │                │               │
  │                 │               │ tools/call     │               │
  │                 │               ├───────────────>│               │
  │                 │               │  ╳ Timeout     │               │
  │                 │               │                │               │
  │                 │               │ Throw          │               │
  │                 │               │ exception      │               │
  │                 │               │                │               │
  │                 │ Handle error  │                │               │
  │                 │<──────────────┤                │               │
  │                 │               │                │               │
  │                 │ Display error │                │               │
  │                 │ message       │                │               │
  │                 │               │                │               │
  │  Error: Failed  │               │                │               │
  │  to communicate │               │                │               │
  │  after 3 retries│               │                │               │
  │<────────────────┤               │                │               │
  │                 │               │                │               │
```

## Component Interaction Diagram

```
┌────────────────────────────────────────────────────────────┐
│                        User Layer                          │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐ │
│  │               Command Line Interface                  │ │
│  │  dotnet run -- [list | add "task" | complete <id>]   │ │
│  └──────────────────────────────────────────────────────┘ │
└────────────────────┬───────────────────────────────────────┘
                     │
                     ▼
┌────────────────────────────────────────────────────────────┐
│                    Application Layer                       │
│                                                            │
│  ┌────────────────────────────────────────────────────┐   │
│  │                   Program.cs                        │   │
│  │  • Configuration loading (User Secrets, Env, JSON)  │   │
│  │  • Command parsing (System.CommandLine)             │   │
│  │  • Routing to command handlers                      │   │
│  └───────────────────┬────────────────────────────────┘   │
│                      │                                     │
│                      ▼                                     │
│  ┌────────────────────────────────────────────────────┐   │
│  │              Command Handlers                       │   │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────┐ │   │
│  │  │ListCommand   │  │AddCommand    │  │Complete  │ │   │
│  │  │              │  │              │  │Command   │ │   │
│  │  └──────────────┘  └──────────────┘  └──────────┘ │   │
│  └───────────────────┬────────────────────────────────┘   │
└──────────────────────┼────────────────────────────────────┘
                       │
                       ▼
┌────────────────────────────────────────────────────────────┐
│                     Service Layer                          │
│                                                            │
│  ┌────────────────────────────────────────────────────┐   │
│  │                   McpClient                         │   │
│  │  • Process management (start/stop MCP server)       │   │
│  │  • Stream handling (stdin/stdout/stderr)            │   │
│  │  • JSON-RPC message formatting                      │   │
│  │  • Request/response correlation                     │   │
│  │  • Retry logic with exponential backoff             │   │
│  │  • Timeout management                               │   │
│  └───────────────────┬────────────────────────────────┘   │
└──────────────────────┼────────────────────────────────────┘
                       │
                       │ JSON-RPC over stdio
                       │
                       ▼
┌────────────────────────────────────────────────────────────┐
│                    MCP Server Layer                        │
│                                                            │
│  ┌────────────────────────────────────────────────────┐   │
│  │        Todoist MCP Server (Node.js)                 │   │
│  │  • JSON-RPC request parsing                         │   │
│  │  • Tool registration (get_tasks, add_task, etc.)    │   │
│  │  • API token handling                               │   │
│  │  • Request forwarding to Todoist API                │   │
│  │  • Response formatting                              │   │
│  └───────────────────┬────────────────────────────────┘   │
└──────────────────────┼────────────────────────────────────┘
                       │
                       │ HTTPS
                       │
                       ▼
┌────────────────────────────────────────────────────────────┐
│                     External API Layer                     │
│                                                            │
│  ┌────────────────────────────────────────────────────┐   │
│  │              Todoist REST API                       │   │
│  │  • GET    /rest/v2/tasks                           │   │
│  │  • POST   /rest/v2/tasks                           │   │
│  │  • POST   /rest/v2/tasks/{id}/complete             │   │
│  │  • Authentication via Bearer token                  │   │
│  └────────────────────────────────────────────────────┘   │
└────────────────────────────────────────────────────────────┘
```

## Message Flow Details

### Initialize Request/Response
```
Client → Server:
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "initialize",
  "params": {
    "protocolVersion": "2024-11-05",
    "capabilities": {},
    "clientInfo": {
      "name": "TodoistMcpConsole",
      "version": "1.0.0"
    }
  }
}

Server → Client:
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "protocolVersion": "2024-11-05",
    "capabilities": {
      "tools": {}
    },
    "serverInfo": {
      "name": "todoist-mcp-server",
      "version": "1.0.0"
    }
  }
}
```

### Tools/Call Request/Response (get_tasks)
```
Client → Server:
{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "tools/call",
  "params": {
    "name": "get_tasks",
    "arguments": {}
  }
}

Server → Client:
{
  "jsonrpc": "2.0",
  "id": 2,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "Found 3 tasks:\n1. [ID: 123] Buy groceries\n2. [ID: 456] Call dentist\n3. [ID: 789] Finish report"
      }
    ]
  }
}
```

### Tools/Call Request/Response (add_task)
```
Client → Server:
{
  "jsonrpc": "2.0",
  "id": 3,
  "method": "tools/call",
  "params": {
    "name": "add_task",
    "arguments": {
      "content": "Buy milk and eggs"
    }
  }
}

Server → Client:
{
  "jsonrpc": "2.0",
  "id": 3,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "Task created successfully with ID: 999"
      }
    ]
  }
}
```

### Tools/Call Request/Response (complete_task)
```
Client → Server:
{
  "jsonrpc": "2.0",
  "id": 4,
  "method": "tools/call",
  "params": {
    "name": "complete_task",
    "arguments": {
      "task_id": "123"
    }
  }
}

Server → Client:
{
  "jsonrpc": "2.0",
  "id": 4,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "Task 123 has been marked as complete."
      }
    ]
  }
}
```

## Timing Considerations

### Typical Response Times
- **Initialize**: 100-500ms
- **List tasks**: 200-1000ms (depends on number of tasks)
- **Add task**: 200-800ms
- **Complete task**: 200-800ms

### Timeout Configuration
- **Connection timeout**: 30 seconds (configurable)
- **Request timeout**: 60 seconds (configurable)
- **Retry delays**: 2s, 4s, 8s (exponential backoff)
- **Maximum retries**: 3 (configurable)

### Performance Optimization
1. **Connection reuse**: McpClient maintains a persistent connection
2. **Minimal overhead**: Direct stdio communication (no HTTP overhead)
3. **Efficient parsing**: Newtonsoft.Json for fast JSON processing
4. **Async operations**: All I/O operations are async
