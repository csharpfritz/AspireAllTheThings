# Mal — History

## Project Context
- **Project:** AspireAllTheThings — a demo suite for an Aspire training session
- **Tech Stack:** .NET Aspire AppHost (C#), ASP.NET Core Web API, Python Flask, Node.js Express, Java Spring Boot
- **User:** Jeffrey T. Fritz
- **Purpose:** Demonstrates official Aspire integrations (Redis, Postgres, SQL Server, Azure services), multi-language support, custom containers (IT-Tools, MailPit), advanced patterns (Discord notifier with eventing), and fun demos (Minecraft)
- **Architecture:** AppHost orchestrates all workloads via `builder.AddXxxDemo()` extension methods defined in numbered files (1-OfficialIntegrations.cs through 6-Fun.cs)
- **Key pattern:** Demos are commented out in AppHost.cs; uncomment to enable

## Learnings
