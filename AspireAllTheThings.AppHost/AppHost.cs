using AspireAllTheThings.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

// ============================================
// CodeStock 2026 - Aspire All The Things!
// "Hooking Up All The Things, Making Your Distributed Developer's Life Easier"
// ============================================
// 
// This session is organized in 7 parts:
// 
// PART 1: Official Aspire Integrations (1-OfficialIntegrations.cs)
//         Redis, PostgreSQL, SQL Server, Azure Service Bus, Cosmos DB, Storage
// 
// PART 2: Multi-Language Support (2-MultiLanguage.cs)
//         ASP.NET, Python Flask, Node.js Express, Java Spring Boot
// 
// PART 3: Custom Integration Creation (3-ItTools.cs)
//         Build your own integrations with Docker containers
// 
// PART 4: MailPit Email Demo (4-MailPit.cs)
//         Community Toolkit integration for local email testing
// 
// PART 5: Advanced Integration Patterns (5-AdvancedIntegrations.cs)
//         Discord Notifier - custom eventing and lifecycle hooks
// 
// PART 6: AI Integration (6-AI.cs)
//         GitHub Models - AI chat with GenAI dashboard visualizer
//
// PART 7: Fun Demos (7-Fun.cs)
//         Minecraft server - because Aspire isn't just for web apps (BONUS!)
//
// Uncomment each demo as you present them!
// ============================================

// ---- PART 1: Official Integrations (1-OfficialIntegrations.cs) ----
builder.AddRedisDemo();           // Redis with Redis Insight
builder.AddPostgresDemo();        // PostgreSQL with pgAdmin
builder.AddSqlServerDemo();       // SQL Server
builder.AddAzureServiceBusDemo(); // Service Bus with emulator
builder.AddAzureCosmosDbDemo();   // Cosmos DB with emulator
builder.AddAzureStorageDemo();    // Blobs, Queues, Tables with Azurite

// // ---- PART 2: Multi-Language Apps (2-MultiLanguage.cs) ----
// builder.AddAspNetApiDemo();       // ASP.NET Core Web API
// builder.AddPythonApiDemo();       // Python Flask API
// builder.AddNodeApiDemo();         // Node.js Express API
// builder.AddJavaApiDemo();				// Java Spring Boot API

// // ---- PART 3: Custom Integration Creation (3-ItTools.cs) ----
// builder.AddItToolsDemo();         // Simple Docker container

// // ---- PART 4: MailPit Email Demo (4-MailPit.cs) ----
// builder.AddMailPitDemo();         // Community Toolkit integration

// // ---- PART 5: Advanced Integration Patterns (5-AdvancedIntegrations.cs) ----
// // Discord Notifier with interactive parameter prompts
// // The dashboard will prompt for webhook URL (secret) and channel name at startup.
// // Or pre-fill via user secrets:
// //   dotnet user-secrets set "Parameters:discordWebhookUrl" "https://discord.com/api/webhooks/..."
// //   dotnet user-secrets set "Parameters:discordChannel" "aspire-demo"
// builder.AddDiscordNotifierDemo();

// // ---- PART 6: AI Integration (6-AI.cs) ----
// // GitHub Models - AI chat with GenAI dashboard visualizer
// // Set "githubApiKey" parameter in user secrets or dashboard prompt
// builder.AddGitHubModelDemo();

// // ---- BONUS: Fun Demos (7-Fun.cs) ----
// builder.AddMinecraftDemo();       // Minecraft server (capstone demo!)

builder.Build().Run();
