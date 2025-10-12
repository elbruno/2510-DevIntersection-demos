using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OllamaSharp;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

// To run the sample, you need to set the following environment variables or user secrets:
// Using GitHub models (for Agent 1):
//      "GITHUB_TOKEN": "your GitHub Token"
// Using Azure Foundry/OpenAI (for Agent 2):
//      "endpoint": "https://<endpoint>.services.ai.azure.com/"
//      "apikey": "your key"
//      "deploymentName": "a deployment name, ie: gpt-4o-mini"
// Ollama should be running locally on http://localhost:11434/ with llama3.2 model (for Agent 3)

Console.WriteLine("=== Microsoft Agent Framework - Multi-Model Orchestration Demo ===");
Console.WriteLine("This demo showcases 3 agents working together:");
Console.WriteLine("  1. Researcher (GitHub Models) - Researches topics");
Console.WriteLine("  2. Writer (Azure Foundry) - Writes content based on research");
Console.WriteLine("  3. Reviewer (Ollama) - Reviews and provides feedback");
Console.WriteLine();

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// ===== Agent 1: Researcher using GitHub Models =====
Console.WriteLine("Setting up Agent 1: Researcher (GitHub Models)...");
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(githubToken))
{
    githubToken = config["GITHUB_TOKEN"];
}

IChatClient githubChatClient =
    new ChatClient(
            "gpt-4o-mini",
            new ApiKeyCredential(githubToken!),
            new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") })
        .AsIChatClient();

AIAgent researcher = new ChatClientAgent(
    githubChatClient,
    new ChatClientAgentOptions
    {
        Name = "Researcher",
        Instructions = "You are a research expert. Your job is to gather key facts and interesting points about the given topic. Be concise and focus on the most important information."
    });

// ===== Agent 2: Writer using Azure Foundry/OpenAI =====
Console.WriteLine("Setting up Agent 2: Writer (Azure Foundry)...");
var endpoint = config["endpoint"] ?? throw new InvalidOperationException("endpoint configuration is required");
var apiKey = new ApiKeyCredential(config["apikey"] ?? throw new InvalidOperationException("apikey configuration is required"));
var deploymentName = !string.IsNullOrEmpty(config["deploymentName"]) ? config["deploymentName"] : "gpt-4o-mini";

IChatClient azureChatClient = 
    new AzureOpenAIClient(
        new Uri(endpoint),
        apiKey)
    .GetChatClient(deploymentName)
    .AsIChatClient();

AIAgent writer = new ChatClientAgent(
    azureChatClient,
    new ChatClientAgentOptions
    {
        Name = "Writer",
        Instructions = "You are a creative writer. Take the research provided and write an engaging, well-structured article. Make it informative yet entertaining."
    });

// ===== Agent 3: Reviewer using Ollama =====
Console.WriteLine("Setting up Agent 3: Reviewer (Ollama)...");
IChatClient ollamaChatClient =
    new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3.2");

AIAgent reviewer = new ChatClientAgent(
    ollamaChatClient,
    new ChatClientAgentOptions
    {
        Name = "Reviewer",
        Instructions = "You are an editor and reviewer. Analyze the article provided, give constructive feedback, and suggest improvements for clarity, grammar, and engagement."
    });

// ===== Create Sequential Workflow =====
Console.WriteLine("Creating workflow: Researcher -> Writer -> Reviewer");
Console.WriteLine();

Workflow workflow =
    AgentWorkflowBuilder
        .BuildSequential(researcher, writer, reviewer);

AIAgent workflowAgent = await workflow.AsAgentAsync();

// ===== Execute the Workflow =====
var topic = "artificial intelligence in healthcare";
Console.WriteLine($"Starting workflow with topic: '{topic}'");
Console.WriteLine(new string('=', 80));
Console.WriteLine();

AgentRunResponse workflowResponse =
    await workflowAgent.RunAsync($"Research and write an article about: {topic}");

Console.WriteLine("=== Final Output ===");
Console.WriteLine(workflowResponse.Text);
Console.WriteLine();
Console.WriteLine(new string('=', 80));
Console.WriteLine("Workflow completed successfully!");
