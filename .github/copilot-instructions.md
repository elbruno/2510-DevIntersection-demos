# GitHub Copilot Instructions — DevIntersection 2025 AI & .NET Demos

## Project Overview

This repository contains demo artifacts for **DevIntersection 2025** sessions covering:
- Model Context Protocol (MCP) integration
- GitHub Copilot and AI APIs with .NET
- Building intelligent .NET applications with AI

The repository includes multiple demo projects showcasing:
- Hugging Face MCP integration (C# and Python)
- AI Web Chat applications using Azure AI Foundry and .NET Aspire
- AgentFx framework samples for AI agents
- Unit testing with GitHub Copilot assistance

## Technology Stack

- **.NET 9 SDK** (primary framework)
- **C# 12** with nullable reference types enabled
- **Python 3.x** for MCP Python samples
- **.NET Aspire** for orchestration and observability
- **Azure AI Foundry** and **Hugging Face** for AI services
- **xUnit** for testing
- **Blazor** for web UI
- **GitHub Codespaces** for development environment

## Code Style and Conventions

### C# Guidelines

- Follow standard C# naming conventions (PascalCase for types/methods, camelCase for locals)
- Use **nullable reference types** — all reference types should be nullable-aware
- Add **XML documentation comments** (`///`) for public classes, methods, and properties
- Use `async`/`await` for all I/O operations
- Prefer explicit types over `var` when the type isn't obvious
- End statements with `;` in C#
- Use modern C# features (pattern matching, records, file-scoped namespaces where appropriate)
- Keep console applications simple and focused on demonstrating specific concepts

### Python Guidelines

- Follow **PEP 8** style guide
- Use type hints for function parameters and return values
- Document functions with docstrings
- Use `snake_case` for variables and functions
- Keep demo scripts minimal and educational

## Project Structure

### Demo Folders

- **1-HFMCP/** — Hugging Face MCP console samples (C#)
  - Minimal, self-contained examples for learning
  - Each demo should have its own `.csproj` and `Program.cs`
  - Include comments explaining the MCP API usage

- **2-HFMCP-python/** — Python MCP demos
  - Simple Python scripts demonstrating MCP integration
  - Include requirements.txt if external packages needed

- **3-AIWebChat/** — AI Web Chat application
  - Contains `1.start/` (starter template) and `2.complete/` (finished demo)
  - Multi-project solution using .NET Aspire patterns
  - Projects: AppHost, ServiceDefaults, Web (Blazor)

- **4-AgentFx/** — AgentFx framework samples
  - Console applications demonstrating AI agent patterns
  - Include workflows, sequential processing, and agent collaboration

- **5-GHCPUnitTests/** — GitHub Copilot assisted unit testing demos
  - .NET Aspire solution with multiple projects
  - Includes test projects with xUnit
  - Demonstrates testing patterns for AI-enabled apps

## Building and Running

### Prerequisites

- .NET 9 SDK installed (https://dotnet.microsoft.com/download/dotnet/9.0)
- Python 3.x (for Python demos)
- Ollama (optional, for local model demos)
- Visual Studio 2022 or VS Code with C# extensions

### Build Commands

```powershell
# Build a specific project
dotnet build <ProjectName>.csproj

# Build a solution
dotnet build <SolutionName>.sln

# Run a console project
dotnet run --project <ProjectName>/<ProjectName>.csproj

# Run tests
dotnet test
```

### Configuration

- Store API keys and secrets in `appsettings.Development.json` or environment variables
- **Never commit** `appsettings.Development.json` files containing secrets
- Use `appsettings.json` for non-sensitive defaults
- Check `Program.cs` comments for required configuration in each sample

## Testing

### Test Framework

- Use **xUnit** for all .NET test projects
- Place tests in separate `*.Tests` projects
- Name test methods descriptively: `MethodName_Scenario_ExpectedBehavior`
- Aim for meaningful test coverage, especially for AI integration code
- Mock external AI service calls when possible to ensure tests are deterministic

### Test Structure

```csharp
[Fact]
public void ServiceMethod_WhenCalled_ReturnsExpectedResult()
{
    // Arrange
    var service = new MyService();
    
    // Act
    var result = service.Method();
    
    // Assert
    Assert.NotNull(result);
}
```

## Security Best Practices

### API Keys and Secrets

- **Never hardcode** API keys, tokens, or connection strings in source code
- Use environment variables or user secrets for local development:
  ```powershell
  dotnet user-secrets set "GITHUB_TOKEN" "your-token-here"
  ```
- Add `*.env`, `appsettings.Development.json` to `.gitignore`
- Reference secrets in `Program.cs` using `IConfiguration` or environment variables
- For demos, include placeholder comments showing where secrets should be configured

### Example Secret Loading Pattern

```csharp
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();
    githubToken = config["GITHUB_TOKEN"];
}
```

## AI Integration Patterns

### Hugging Face & Azure AI Foundry

- Use `Microsoft.Extensions.AI` abstractions for AI client interactions
- Create `IChatClient` instances for chat completions
- Use `ChatClientAgent` from `Microsoft.Agents.AI` for agent-based patterns
- Handle rate limiting and API errors gracefully
- Log AI interactions for debugging (but exclude from production)

### Agent Patterns

- Use `AIAgent` abstraction from Microsoft.Agents.AI
- Configure agents with clear `Name` and `Instructions`
- Chain agents using `AgentWorkflowBuilder.BuildSequential()` for workflows
- Keep agent instructions focused and specific
- Test agent interactions with various prompts

## Documentation

### Code Comments

- Add XML documentation for public APIs
- Use inline comments sparingly — prefer self-documenting code
- Document **why** not **what** when code isn't obvious
- Include usage examples in XML comments for complex methods

### README Files

- Each major demo folder should have its own `README.md`
- Include: Purpose, How to Run, Prerequisites, Configuration
- Provide copy-paste ready commands for common tasks
- Link to relevant external documentation

## Demo Code Guidelines

Since this is a demo repository:

- **Prioritize clarity over optimization** — demos should be easy to understand
- **Keep samples minimal** — focus on one concept per demo
- **Include console output examples** when relevant
- **Comment liberally** to explain what's happening
- **Use descriptive variable names** that explain their purpose
- **Show best practices** that attendees can apply in their own projects

## .NET Aspire Specifics

- Use `IDistributedApplicationBuilder` for orchestration in AppHost projects
- Configure service defaults in ServiceDefaults projects
- Use dependency injection throughout
- Leverage built-in observability (logging, metrics, tracing)
- Follow Aspire project structure conventions

## GitHub Codespaces

- Ensure projects work in Codespaces without local dependencies
- Include `.devcontainer/devcontainer.json` if custom setup needed
- Use .NET 9 SDK in container configuration
- Pre-install VS Code extensions: C#, GitHub Copilot, Azure Tools

## Common Patterns to Follow

### Async/Await

```csharp
// Always use async for I/O
public async Task<string> GetDataAsync()
{
    var result = await client.GetAsync("endpoint");
    return await result.Content.ReadAsStringAsync();
}
```

### Dependency Injection

```csharp
// Register services
builder.Services.AddSingleton<IChatClient>(chatClient);

// Inject in constructors
public MyService(IChatClient chatClient)
{
    _chatClient = chatClient;
}
```

### Error Handling

```csharp
try
{
    var response = await agent.RunAsync(prompt);
    Console.WriteLine(response.Text);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    // Handle gracefully for demos
}
```

## Files to Ignore

Already configured in `.gitignore`:
- `bin/`, `obj/` directories
- `*.user`, `*.suo` files
- `*.env` files
- `appsettings.Development.json`
- `.vs/`, `.vscode/` (except approved settings)
- `node_modules/`

## Getting Help

- Check individual project `README.md` files
- Review `Program.cs` comments for configuration guidance
- Refer to main repository `Readme.md` for overall structure
- For complete working examples, see: https://github.com/elbruno/OrlandoParksLabs

## When Generating New Code

- Use .NET 9 SDK features and patterns
- Follow the existing project structure
- Add appropriate XML documentation
- Include configuration examples
- Create corresponding test projects when adding new functionality
- Keep demos focused and educational
- Add clear console output or logging
- Handle errors gracefully with user-friendly messages
