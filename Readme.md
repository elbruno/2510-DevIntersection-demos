# Elevate Your .NET with GitHub Copilot and AI APIs — DevIntersection Demos

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
2. Ensure the container uses **.NET 9 SDK** (download locally if needed: [dotnet.microsoft.com/en-us/download/dotnet/9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0))
3. Explore folders such as:
   - `/prompts` → Copilot and Codespaces prompts  
   - `/snippets` → code used live  
   - `/slides` → reference slides or visuals  

---

## 🧑‍💻 Reproduce the Copilot Agent + Codespaces setup

Try these prompts inside **GitHub Copilot Chat** (Agent Mode) with repo context enabled:

```text
@workspace I want to run this project directly in GitHub Codespaces.
Please add all the necessary configuration files so that the repo automatically builds and runs when a Codespace starts.
Use .NET 9 SDK, configure a devcontainer.json, and include VS Code extensions for C#, GitHub Copilot, and Azure Tools.
