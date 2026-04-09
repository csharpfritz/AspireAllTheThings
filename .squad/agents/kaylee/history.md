# Kaylee — History

## Project Context
- **Project:** AspireAllTheThings — Aspire training demo suite
- **Tech Stack:** .NET Aspire AppHost (C#), ASP.NET Core Web API, Aspire SDK 13.1
- **User:** Jeffrey T. Fritz
- **Key packages:** Aspire.Hosting.Redis, Aspire.Hosting.PostgreSQL, Aspire.Hosting.SqlServer, Aspire.Hosting.Azure.ServiceBus, Aspire.Hosting.Azure.CosmosDB, Aspire.Hosting.Azure.Storage, CommunityToolkit.Aspire.Hosting.Mailpit
- **Pattern:** Each demo part is a static class with `builder.AddXxxDemo()` extension methods
- **AppHost uses:** `Aspire.AppHost.Sdk/13.1.0`

## Learnings
- `Aspire.Hosting.NodeJs` was deprecated/renamed to `Aspire.Hosting.JavaScript` in Aspire 13.x; `AddNpmApp` → `AddJavaScriptApp` with `runScriptName:` parameter
- The demo has 7 parts (not 6): Official Integrations, Multi-Language, IT-Tools, MailPit, Discord, **AI Integration (Part 6)**, **Fun/Minecraft Capstone (Part 7)**
- Part numbering in AppHost.cs inline comments must match the README table and file naming (3-ItTools.cs = Part 3, 4-MailPit.cs = Part 4, etc.)
- All first-party Aspire packages should stay at the same version (currently 13.1.0); CommunityToolkit packages may differ slightly (13.1.1)
- `Aspire.Hosting.GitHub.Models` (v13.1.0) is the hosting package for GitHub Models AI integration; uses `AddGitHubModel("name", "openai/gpt-4o-mini")` API
- The Aspire AI client packages (`Aspire.Azure.AI.Inference`, `Aspire.OpenAI`) are preview-only as of July 2025; use exact preview version strings (e.g., `13.1.1-preview.1.26105.8`)
- GitHub Models client registration: `builder.AddAzureChatCompletionsClient("chat").AddChatClient()` registers `IChatClient` for DI
- `AddParameter("name", secret: true)` triggers the Aspire dashboard's interactive parameter prompt feature — great for API keys
- The GenAI visualizer in the Aspire dashboard appears automatically when AI telemetry flows through OpenTelemetry; no extra config needed
- Part 6 (AI Integration) includes the webapi project registration with `.WithReference(chat)` — don't uncomment both Part 2's `AddAspNetApiDemo()` and Part 6 simultaneously (resource name conflict)
- **2026-04-09:** Reordered Parts 6 & 7 — AI Integration is now Part 6, Fun/Minecraft is now Part 7 (capstone). Files renamed via `git mv`, all comments updated, AppHost.cs header reordered, README updated. Build verified clean. User directive from 2026-04-09T13:46 to keep Minecraft as final demo drove this change.
- The Part 6 AI demo flow: show 6-AI.cs (15 lines), show WebApi client registration + `/chat` endpoint, uncomment in AppHost.cs, run, show dashboard parameter prompt (secret: true), test `/chat`, show GenAI visualizer traces. Key conflict: Part 2 and Part 6 both register `webapi` resource — never uncomment both simultaneously.
- WebApi/Program.cs still has stale "Part 7" references in comments (lines 11, 48) from the reorder — cosmetic only, code is correct. Should be cleaned up in a future pass.
- GitHub Models requires internet (`models.inference.ai.azure.com`) — venue Wi-Fi is a prerequisite for live demos.
- **2026-04-09 (Chat UI):** Part 6 AI demo now has a proper chat UI at `/chat.html` served via `app.UseStaticFiles()`. The `/chat` endpoint changed from GET with query string to POST with JSON body `{ "message": "..." }`. Chat UI is self-contained (single HTML file, inline CSS/JS), dark theme matching Aspire dashboard, includes loading indicator and error handling. Demo script updated to reference browsing to chat UI instead of raw endpoint. Files: `AspireAllTheThings.WebApi/wwwroot/chat.html`, `Program.cs` (added UseStaticFiles, changed endpoint to POST, added ChatRequest record), `.squad/files/part6-ai-demo-script.md` (Step 6 updated).
- ASP.NET Core static files: `app.UseStaticFiles()` must be called after `app.Build()` but before `app.MapGet/MapPost` — serves files from `wwwroot/` at root path (e.g., `wwwroot/chat.html` → `/chat.html`)
- Top-level statements in Program.cs: record types must come **after** `app.Run()`, not before — C# requirement for top-level program structure
