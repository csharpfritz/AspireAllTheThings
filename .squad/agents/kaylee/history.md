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
- The demo has 6 parts (not 3 or 5): Official Integrations, Multi-Language, IT-Tools, MailPit, Discord, Minecraft
- Part numbering in AppHost.cs inline comments must match the README table and file naming (3-ItTools.cs = Part 3, 4-MailPit.cs = Part 4)
- All first-party Aspire packages should stay at the same version (currently 13.1.0); CommunityToolkit packages may differ slightly (13.1.1)
