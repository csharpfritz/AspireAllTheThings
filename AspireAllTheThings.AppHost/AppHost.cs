using AspireAllTheThings.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

// ============================================
// CodeStock 2026 - Aspire All The Things!
// "Hooking Up All The Things, Making Your Distributed Developer's Life Easier"
// ============================================
// 
// This session is organized in 6 parts:
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
// PART 6: Fun Demos (6-Fun.cs)
//         Minecraft server - because Aspire isn't just for web apps
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
// // Discord Notifier - Posts to Discord when resources change state
// // Set "Discord:WebhookUrl" in user secrets to enable:
// //   dotnet user-secrets set "Discord:WebhookUrl" "https://discord.com/api/webhooks/..."
// builder.AddDiscordNotifierDemo();

// // ---- BONUS: Fun Demos (6-Fun.cs) ----
// builder.AddMinecraftDemo();       // Minecraft server 

builder.Build().Run();
