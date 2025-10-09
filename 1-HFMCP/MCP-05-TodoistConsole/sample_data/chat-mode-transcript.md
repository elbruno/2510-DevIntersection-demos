# Chat Mode - Sample Transcript

This document contains example interactions using the interactive chat mode with AI integration.

## Starting Chat Mode

### Command
```bash
dotnet run -- chat
```

### Initial Output
```
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

---

## Example Session 1: Viewing Tasks

```
You: Show me all my tasks
  [Calling tool: get_tasks]
Assistant: Here are your current tasks from Todoist:

1. [ID: 7234567890] Buy groceries for the week
   Project: Personal | Priority: 3 | Due: Today

2. [ID: 7234567891] Finish quarterly report
   Project: Work | Priority: 4 | Due: Tomorrow

3. [ID: 7234567892] Schedule dentist appointment
   Project: Health | Priority: 2 | Due: This week

You have 3 tasks in total. Would you like me to help you with any of them?
```

---

## Example Session 2: Adding Tasks

```
You: I need to add a task to prepare a demo for the DevIntersection conference
  [Calling tool: add_task]
Assistant: Perfect! I've added the task "Prepare a demo for the DevIntersection conference" to your Todoist. 
The task has been created successfully with ID: 7234567899.

You: Can you also add a reminder to review the slides?
  [Calling tool: add_task]
Assistant: Done! I've added "Review the slides" to your tasks. 
Task ID: 7234567900

You: Thanks!
Assistant: You're welcome! You now have 5 tasks total. Is there anything else you'd like to do?
```

---

## Example Session 3: Completing Tasks

```
You: I finished buying groceries, can you mark task 7234567890 as complete?
  [Calling tool: complete_task]
Assistant: Excellent! I've marked task 7234567890 (Buy groceries for the week) as complete. 
Great job getting that done! 🎉

You: What tasks do I have left?
  [Calling tool: get_tasks]
Assistant: Here are your remaining tasks:

1. [ID: 7234567891] Finish quarterly report
   Project: Work | Priority: 4 | Due: Tomorrow

2. [ID: 7234567892] Schedule dentist appointment
   Project: Health | Priority: 2 | Due: This week

3. [ID: 7234567899] Prepare a demo for the DevIntersection conference
   Project: Inbox

4. [ID: 7234567900] Review the slides
   Project: Inbox

You have 4 tasks remaining. Keep up the good work!
```

---

## Example Session 4: Natural Language Queries

```
You: What do I need to do today?
  [Calling tool: get_tasks]
Assistant: Looking at your tasks, you have one task due today:

1. [ID: 7234567890] Buy groceries for the week
   Project: Personal | Priority: 3 | Due: Today

This is your priority for today. Would you like me to help with anything else?

You: What about this week?
Assistant: Based on your current tasks, you have one task due this week:

1. [ID: 7234567892] Schedule dentist appointment
   Project: Health | Priority: 2 | Due: This week

You also have the urgent task due tomorrow (Finish quarterly report). Would you like to work on any of these?

You: Let me complete the groceries task
  [Calling tool: complete_task]
Assistant: Great! Task 7234567890 (Buy groceries for the week) is now marked as complete. 
Well done! 🎉
```

---

## Example Session 5: Conversational Flow

```
You: Hi! I'm planning my week
Assistant: Hello! I'd be happy to help you plan your week. Would you like me to show you your current tasks?

You: Yes please
  [Calling tool: get_tasks]
Assistant: Here's what you have on your plate:

1. [ID: 7234567891] Finish quarterly report (Due: Tomorrow, Priority: High)
2. [ID: 7234567892] Schedule dentist appointment (Due: This week)
3. [ID: 7234567899] Prepare a demo for the DevIntersection conference
4. [ID: 7234567900] Review the slides

You have 4 tasks. The quarterly report due tomorrow should probably be your top priority. How would you like to proceed?

You: Good point. Can you add a task to send the report to my manager?
  [Calling tool: add_task]
Assistant: I've added "Send the report to my manager" to your tasks (ID: 7234567901). 
This is a good follow-up task for after you finish the quarterly report!

You: Perfect! I'll work on these now
Assistant: Excellent! Good luck with your tasks! Let me know if you need any help managing them. 
You can always come back to chat mode to update or check your progress.

You: Thanks! Bye
Assistant: You're welcome! Have a productive week! 👋
```

---

## Example Session 6: Error Handling

```
You: Complete task 999999999
  [Calling tool: complete_task]
Assistant: I encountered an issue trying to complete task 999999999. 
The task ID doesn't seem to exist in your Todoist. 
Would you like me to show you your current tasks so you can find the correct ID?

You: Yes
  [Calling tool: get_tasks]
Assistant: Here are your current tasks with their IDs:

1. [ID: 7234567891] Finish quarterly report
2. [ID: 7234567892] Schedule dentist appointment
3. [ID: 7234567899] Prepare a demo for the DevIntersection conference
4. [ID: 7234567900] Review the slides

Which one would you like to complete?

You: The dentist one - 7234567892
  [Calling tool: complete_task]
Assistant: Perfect! Task 7234567892 (Schedule dentist appointment) is now marked as complete. ✓
```

---

## Example Session 7: Exiting Chat Mode

```
You: I'm done for now
Assistant: Is there anything else I can help you with before you go?

You: No, I'm good. Thanks!
Assistant: You're welcome! Have a great day!

You: exit

Goodbye! Thanks for using Todoist MCP Chat! 👋
```

---

## Notes on Chat Mode

### Key Features

1. **Natural Language Understanding**: The AI understands intent without requiring exact command syntax
   - "Show me my tasks" → calls get_tasks
   - "Add a task to..." → calls add_task
   - "Mark task X as done" → calls complete_task

2. **Contextual Awareness**: The assistant maintains conversation context and can reference previous interactions

3. **Tool Transparency**: You can see when MCP tools are being called with `[Calling tool: tool_name]` indicators

4. **Friendly Interaction**: The AI provides helpful, conversational responses rather than just raw data

5. **Error Recovery**: When something goes wrong, the AI helps you understand and correct the issue

### Tips for Best Experience

- **Be Natural**: Talk to the assistant as you would to a colleague
- **Be Specific**: When referencing tasks, use task IDs when possible
- **Ask Questions**: The AI can help clarify what you need to do
- **Use Context**: Reference previous messages in the conversation

### Configuration Requirements

Chat mode requires Azure OpenAI configuration:

```bash
dotnet user-secrets set "endpoint" "https://your-endpoint.openai.azure.com/"
dotnet user-secrets set "apikey" "your-api-key"
dotnet user-secrets set "deploymentName" "gpt-4o-mini"  # Optional
```

### Performance

- **Response Time**: Typically 1-3 seconds for simple queries
- **Tool Calls**: Add 1-2 seconds per tool invocation
- **Token Usage**: Varies based on conversation length and complexity

---

## Comparison: Direct Commands vs Chat Mode

### Direct Commands (Traditional CLI)
```bash
dotnet run -- list
dotnet run -- add "Buy groceries"
dotnet run -- complete 123456
```
- **Pros**: Fast, scriptable, no AI dependency
- **Cons**: Requires exact syntax, one command at a time

### Chat Mode (AI-Powered)
```bash
dotnet run -- chat
You: Show my tasks and help me plan my day
```
- **Pros**: Natural language, conversational, contextual
- **Cons**: Requires Azure OpenAI, slightly slower, uses API credits

Both modes are available - choose based on your needs!
