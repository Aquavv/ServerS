# ServerS - Agent Directives (AGENTS.md)

Welcome! This file contains the core guidelines and contextual rules for any AI agent or developer working on the **ServerS** project. It is designed to be a living document—easily modifiable and expandable. If you introduce new patterns or technologies to the project, please update this document accordingly.

## 📌 1. Project Context
**ServerS** is a lightweight, high-performance desktop application built with AvaloniaUI (.NET 10.0). 
Its primary goal is to manipulate Windows/Linux firewall rules to block high-ping datacenters for games like **CS2, Deadlock, and Overwatch 2**, ensuring players connect to optimal local servers.

- **Architecture:** MVVM Pattern (Model-View-ViewModel) using `CommunityToolkit.Mvvm`.
- **Dependency Injection:** Centralized IoC container via `Microsoft.Extensions.DependencyInjection` configured in `App.axaml.cs`.
- **Distribution:** Compiled as a single-file, self-contained executable.

## 🚀 2. Build & Publish Commands
Whenever you need to build or publish the application, use these exact commands from the repository root:

*   **Clean Workspace:** `dotnet clean ServerS.slnx`
*   **Debug/Development Build:** `dotnet build ServerS.slnx -c Debug`
*   **Publish (Windows):** `dotnet publish ServerS.slnx -c Release -r win-x64 -p:PublishSingleFile=true`
*   **Publish (Linux):** `dotnet publish ServerS.slnx -c Release -r linux-x64 -p:PublishSingleFile=true`
*   **Build Installer:** Requires Inno Setup 6. Run `iscc ServerS/setup.iss`.

## 🧪 3. Testing & Linting
We prioritize stability. Ensure your code passes formatting and testing before committing.

*   **Run Unit Tests:** `dotnet test ServerS.Tests.slnx`
*   **Check Formatting:** `dotnet format ServerS.slnx --verify-no-changes`
*   **Apply Formatting (Ask User First):** `dotnet format ServerS.slnx`

*When creating new tests:* Place them in the `ServerS.Tests` project using **xUnit** and target `net10.0`.

## ✍️ 4. Coding Standards & Style
To maintain a clean and uniform codebase, strictly adhere to the following rules:

### C# Conventions
*   **Async/Await:** Always return `Task` or `Task<T>`. Suffix asynchronous method names with `Async`. Only use the `async` keyword when `await` is present inside the body.
*   **Exception Handling:** Avoid silent failures. Catch specific exceptions. Log errors via `FileLoggerService` and surface critical ones to the user via `MessageBoxService`.
*   **Naming Conventions:**
    *   `PascalCase` for public members (Classes, Methods, Properties).
    *   `_camelCase` for private fields (e.g., `_loggerService`).
    *   `camelCase` for local variables.
*   **Imports:** Keep `using` directives at the top, sorted alphabetically. System namespaces come first.

### Avalonia / MVVM Conventions
*   **ViewModels:** Must inherit from `ObservableObject`. Use `[ObservableProperty]` for reactive UI bindings. Keep UI-specific references out of ViewModels.
*   **Views (XAML):** Keep code-behind files (`.axaml.cs`) absolutely minimal. UI logic belongs in the ViewModel unless it directly manipulates the raw visual tree (like custom animations or dynamic tooltips).
*   **Assets:** Store images/icons in `ServerS/Assets/` and reference them using Avalonia pack URIs.
*   **Styling:** Abstract repeated UI elements into dictionaries inside `ServerS/Styles/` and link them in `App.axaml`.

## 🤖 5. Agent Instructions & Rules
This project was **entirely coded by a human**. AI was strictly used as a supportive tool for formatting and assistance, not for generating the core logic. 

If you are an AI assistant interacting with this repository, adhere strictly to these operational constraints:

1.  **Avoid Reasoning Loops:** If you repeat a failed strategy across 3 conversational turns, stop immediately and ask the user for clarification.
2.  **Use Context:** Utilize search tools to look up Avalonia documentation or project references when unsure of an implementation detail.
3.  **Do Not Assume Paths:** Always verify directories before creating files or running commands.
