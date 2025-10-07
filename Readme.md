# Elevate Your .NET with GitHub Copilot and AI APIs — DevIntersection Demos

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Contributors](https://img.shields.io/github/contributors/elbruno/2510-DevIntersection-demos.svg)](https://github.com/elbruno/2510-DevIntersection-demos/graphs/contributors)
[![Issues](https://img.shields.io/github/issues/elbruno/2510-DevIntersection-demos.svg)](https://github.com/elbruno/2510-DevIntersection-demos/issues)
[![Pull Requests](https://img.shields.io/github/issues-pr/elbruno/2510-DevIntersection-demos.svg)](https://github.com/elbruno/2510-DevIntersection-demos/pulls)
[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?repo=elbruno/2510-DevIntersection-demos)

---

### 💬 Join the Conversation

- 💡 **[Azure AI Foundry Discussions](https://github.com/Azure/azure-ai-foundry/discussions)**  
- 💬 **[Azure AI Discord Community](https://discord.gg/azureai)**  

---

> Slides and demo artifacts for the session **“Elevate Your .NET with GitHub Copilot and AI APIs.”**  
> If you’re looking for the **finished, runnable sample** created live from prompts, head over to:  
> 👉 **[OrlandoParksLabs](https://github.com/elbruno/OrlandoParksLabs)**

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?repo=elbruno/2510-DevIntersection-demos)

---

## 🧩 What’s in this repo?

This repository contains the lightweight assets used during the **DevIntersection 2025** session:

- 🎤 **Slides & session flow:** from Copilot to .NET AI integration and deployment  
- 💻 **Starter code & snippets:** samples demonstrated live  
- 🤖 **Demo prompts:** the same ones used on stage with GitHub Copilot  
- 🔗 **Links** to the completed app and official Microsoft AI/.NET resources

If you want a **fully working end-to-end implementation**, check out the repo below:  
➡️ **[OrlandoParksLabs](https://github.com/elbruno/OrlandoParksLabs)**

---

## 🚀 Demos covered

1. **Copilot x PowerPoint**  
   Starting with Copilot-generated slides and refining them with Designer.

2. **Creating a repo with custom instructions (GitHub + Copilot)**  
   Building an interactive console game about Orlando Fun Parks.

3. **Adding AI features to .NET**  
   Using Azure AI Foundry and Hugging Face MCP APIs through C# SDKs.

4. **Orchestrating with .NET Aspire**  
   Running locally with observability, then deploying to Azure.

5. **Copilot Coding Agent + Codespaces**  
   Asking Copilot to generate all necessary configuration to run in GitHub Codespaces.

> 💡 The **final, polished version** of all demos lives at  
> **[OrlandoParksLabs](https://github.com/elbruno/OrlandoParksLabs)**

---

## 🧠 Quick start (this repo)

You can explore this repo directly in **GitHub Codespaces**:

1. Click the **Open in GitHub Codespaces** badge above  
   or open: [https://github.com/codespaces/new?repo=elbruno/2510-DevIntersection-demos](https://github.com/codespaces/new?repo=elbruno/2510-DevIntersection-demos)
3. Ensure the container uses **.NET 9 SDK** (download locally if needed: [dotnet.microsoft.com/en-us/download/dotnet/9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0))
4. Explore folders such as:
   - `/prompts` → Copilot and Codespaces prompts  
   - `/snippets` → code used live  
   - `/slides` → reference slides or visuals  
   - `/2-AIWebChat/1.start` → starter project for AI Web Chat  
   - `/2-AIWebChat/2.complete` → completed AI Web Chat project
   - `/1-HFMCP/` → console samples showing Hugging Face MCP usage from C#
     - `/1-HFMCP/MCP-01-HuggingFace/` → minimal Hugging Face MCP console app
     - `/1-HFMCP/MCP-02-HuggingFace-Ollama/` → Ollama/Hugging Face example console app

   ---

   ## Folder details

   Below are brief explanations of the two main demo folders so you can quickly understand what to open and run.

   1-HFMCP
   - Purpose: contains small .NET console samples that demonstrate calling Hugging Face MCP (Model Control Plane) style APIs from C#. These are minimal, self-contained examples intended for learning and quick experimentation.
   - Notable folders/files:
      - `MCP-01-HuggingFace/` — a tiny console app (`MCP-01-HuggingFace.csproj`, `Program.cs`) showing the simplest pattern to call a hosted model or inference endpoint.
      - `MCP-02-HuggingFace-Ollama/` — an example that wires an Ollama local model or Ollama-style runtime through the same patterns used for cloud-hosted endpoints. Also includes `MCP-02-HuggingFace-Ollama.csproj` and `Program.cs`.
   - How to run: open the folder in an editor or run from the command line with the .NET SDK installed (recommended .NET 9). Typical commands:

      ```powershell
      cd 1-HFMCP\MCP-01-HuggingFace
      dotnet run
      ```

      Replace the path with `MCP-02-HuggingFace-Ollama` to run the second sample. Add any required API keys or local model runtime configuration into `appsettings.json` or environment variables as noted in each sample's README or `Program.cs` comments.

   2-AIWebChat
   - Purpose: a multi-project sample (AppHost, ServiceDefaults, Web) that demonstrates an end-to-end AI Web Chat application using Azure AI Foundry patterns and/or Hugging Face integrations. The `1.start` folder is a trimmed starter ready for follow-along demos; `2.complete` contains the finished version used for demos.
   - Notable folders/files:
      - `AIWebChat-HF-AIFoundry.AppHost/` — host project for background services and configuration (`AppHost.cs`, `appsettings*.json`).
      - `AIWebChat-HF-AIFoundry.ServiceDefaults/` — shared service wiring and extension helpers.
      - `AIWebChat-HF-AIFoundry.Web/` — the Blazor/ASP.NET Web UI (`Program.cs`, `Components/`, `wwwroot/`, `Services/`).
   - How to run: open the solution in the `1.start` or `2.complete` folder with Visual Studio or run from the command line using the .NET SDK. Example (from `1.start`):

      ```powershell
      cd 2-AIWebChat\1.start\AIWebChat-HF-AIFoundry.Web
      dotnet run
      ```

      Or open the solution `AIWebChat-HF-AIFoundry.slnx` in Visual Studio/VS Code and run the Web project. Check each project's `appsettings.Development.json` for any API keys, endpoints, or local model configuration required to connect to AI services.

   Notes & tips
   - Use .NET 9 SDK to build and run the projects (links earlier in this README). If you're using Codespaces or a devcontainer, ensure the container has .NET 9 installed.
   - Many samples expect you to provide credentials or local runtimes (Ollama) via environment variables or `appsettings` — inspect `Program.cs` comments for guidance.
   - The `2.complete` folder contains the finished version that was used in the live demo; if you want a reference implementation, start there.

---

## 🧑‍💻 Reproduce the Copilot Agent + Codespaces setup

Try these prompts inside **GitHub Copilot Chat** (Agent Mode) with repo context enabled:

```text
@workspace I want to run this project directly in GitHub Codespaces.
Please add all the necessary configuration files so that the repo automatically builds and runs when a Codespace starts.
Use .NET 9 SDK, configure a devcontainer.json, and include VS Code extensions for C#, GitHub Copilot, and Azure Tools.
