# Sample Command Transcripts

This document contains example outputs from running various commands in the Todoist MCP Console application.

## Table of Contents

1. [List Tasks](#list-tasks)
2. [Add Task](#add-task)
3. [Complete Task](#complete-task)
4. [List MCP Tools](#list-mcp-tools)
5. [Error Scenarios](#error-scenarios)

---

## List Tasks

### Command
```bash
dotnet run -- list
```

### Output
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Fetching your tasks from Todoist...

=== Your Todoist Tasks ===
Found 5 tasks:

1. [ID: 7234567890] Buy groceries for the week
   Project: Personal | Priority: 3 | Due: Today

2. [ID: 7234567891] Finish quarterly report
   Project: Work | Priority: 4 | Due: Tomorrow

3. [ID: 7234567892] Schedule dentist appointment
   Project: Health | Priority: 2 | Due: This week

4. [ID: 7234567893] Review pull requests
   Project: Work | Priority: 3 | Due: Today

5. [ID: 7234567894] Plan weekend trip
   Project: Personal | Priority: 1 | No due date
```

### Alternative Output (Empty List)
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Fetching your tasks from Todoist...

=== Your Todoist Tasks ===
No tasks found. You're all caught up! 🎉
```

---

## Add Task

### Command
```bash
dotnet run -- add "Buy milk and eggs from the store"
```

### Output
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Adding task: 'Buy milk and eggs from the store'...
✓ Task added successfully!

Task details:
  ID: 7234567895
  Title: Buy milk and eggs from the store
  Project: Inbox
  Created: 2025-01-15 10:30:45
```

### Additional Examples

#### Adding a task with special characters
```bash
dotnet run -- add "Read \"The Pragmatic Programmer\" book"
```

Output:
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Adding task: 'Read "The Pragmatic Programmer" book'...
✓ Task added successfully!

Task details:
  ID: 7234567896
  Title: Read "The Pragmatic Programmer" book
  Project: Inbox
  Created: 2025-01-15 10:35:20
```

#### Adding a longer task description
```bash
dotnet run -- add "Prepare presentation for DevIntersection conference including demos and slides"
```

Output:
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Adding task: 'Prepare presentation for DevIntersection conference including demos and slides'...
✓ Task added successfully!

Task details:
  ID: 7234567897
  Title: Prepare presentation for DevIntersection conference including demos and slides
  Project: Inbox
  Created: 2025-01-15 10:40:15
```

---

## Complete Task

### Command
```bash
dotnet run -- complete 7234567890
```

### Output
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Marking task 7234567890 as complete...
✓ Task marked as complete!

Task 7234567890 (Buy groceries for the week) has been completed.
Completed at: 2025-01-15 11:00:00
```

### Alternative Output
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Marking task 7234567891 as complete...
✓ Task marked as complete!

Great job! Task "Finish quarterly report" is now done! 🎯
```

---

## List MCP Tools

### Command
```bash
dotnet run -- tools
```

### Output
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Connecting to Todoist MCP server...

=== Available MCP Tools ===
  • get_tasks
  • add_task
  • complete_task
  • update_task
  • delete_task
  • get_projects
  • add_project
```

---

## Error Scenarios

### 1. Missing API Token

#### Command
```bash
dotnet run -- list
```

#### Output (when TODOIST_API_TOKEN is not set)
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Error: TODOIST_API_TOKEN not found!

Please set your Todoist API token using one of the following methods:
  1. User Secrets: dotnet user-secrets set "TODOIST_API_TOKEN" "your-token"
  2. Environment Variable: export TODOIST_API_TOKEN=your-token
  3. appsettings.json: Add "TodoistApiToken": "your-token"
```

### 2. Empty Task Title

#### Command
```bash
dotnet run -- add ""
```

#### Output
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Error: Task title cannot be empty.
```

### 3. Invalid Task ID

#### Command
```bash
dotnet run -- complete 999999999999
```

#### Output
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Marking task 999999999999 as complete...
Error completing task: Task with ID 999999999999 not found.
```

### 4. Network Timeout (with Retries)

#### Command
```bash
dotnet run -- list
```

#### Output (when network is slow/unavailable)
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Fetching your tasks from Todoist...
Request timed out. Retrying... (Attempt 1 of 3)
Request timed out. Retrying... (Attempt 2 of 3)
Request timed out. Retrying... (Attempt 3 of 3)
Error listing tasks: Failed to communicate with MCP server after 3 retries.

Troubleshooting tips:
  • Check your internet connection
  • Verify your Todoist API token is valid
  • Try increasing timeout values in appsettings.json
```

### 5. MCP Server Not Found

#### Command
```bash
dotnet run -- list
```

#### Output (when Node.js/npm is not installed)
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Error: Failed to start MCP server process.

The MCP server requires Node.js and npm to be installed.
Please install Node.js from: https://nodejs.org/

You can verify installation by running:
  node --version
  npm --version
```

### 6. Invalid Command

#### Command
```bash
dotnet run -- delete 123
```

#### Output
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Unrecognized command or argument 'delete'

Available commands:
  list                List all tasks from Todoist
  add <title>         Add a new task to Todoist
  complete <id>       Mark a task as complete
  tools               List available MCP tools

For more information, run: dotnet run -- --help
```

### 7. Help Command

#### Command
```bash
dotnet run -- --help
```

#### Output
```
Description:
  Todoist MCP Console - Manage your Todoist tasks via MCP

Usage:
  TodoistMcpConsole [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  list             List all tasks from Todoist
  add <title>      Add a new task to Todoist
  complete <id>    Mark a task as complete
  tools            List available MCP tools
```

---

## Full Workflow Example

### Scenario: Adding and Completing a Task

```bash
# Step 1: List current tasks
$ dotnet run -- list
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Fetching your tasks from Todoist...

=== Your Todoist Tasks ===
Found 2 tasks:

1. [ID: 7234567890] Buy groceries
2. [ID: 7234567891] Call dentist

# Step 2: Add a new task
$ dotnet run -- add "Prepare demo for conference"
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Adding task: 'Prepare demo for conference'...
✓ Task added successfully!

Task details:
  ID: 7234567898
  Title: Prepare demo for conference
  Created: 2025-01-15 12:00:00

# Step 3: List tasks again to verify
$ dotnet run -- list
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Fetching your tasks from Todoist...

=== Your Todoist Tasks ===
Found 3 tasks:

1. [ID: 7234567890] Buy groceries
2. [ID: 7234567891] Call dentist
3. [ID: 7234567898] Prepare demo for conference ← NEW

# Step 4: Complete a task
$ dotnet run -- complete 7234567890
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Marking task 7234567890 as complete...
✓ Task marked as complete!

Task 7234567890 (Buy groceries) has been completed.

# Step 5: Verify completion
$ dotnet run -- list
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Fetching your tasks from Todoist...

=== Your Todoist Tasks ===
Found 2 tasks:

1. [ID: 7234567891] Call dentist
2. [ID: 7234567898] Prepare demo for conference
```

---

## Performance Examples

### Fast Response (Good Network)
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Fetching your tasks from Todoist...
[Response time: 234ms]

=== Your Todoist Tasks ===
Found 10 tasks:
...
```

### Slow Response (Poor Network, with Retry)
```
╔════════════════════════════════════════╗
║     Todoist MCP Console v1.0.0         ║
║   Manage tasks with MCP integration    ║
╚════════════════════════════════════════╝

Fetching your tasks from Todoist...
[Request timed out after 60s]
Retrying... (Attempt 1 of 3) [Waiting 2s]
[Response time: 1,523ms]

=== Your Todoist Tasks ===
Found 10 tasks:
...
```

---

## Notes

- All timestamps are in ISO 8601 format
- Task IDs are numeric values provided by the Todoist API
- Colors in actual output: Cyan (status messages), Green (success), Red (errors), Yellow (warnings)
- Response times vary based on network conditions and Todoist API performance
- The MCP server is automatically started and stopped for each command
