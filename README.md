# Hooking Up All The Things - Making Your Distributed Developer's Life Easier

**Swetugg Stockholm 2026** | February 4, 12:50 | Liljeholmssalen

> Distributed applications are powerful, but also complex to stitch together.
> Between service discovery, configuration, observability, and deployment, developers
> spend more time wiring up infrastructure than building features. Enter Aspire!

## Session Overview

This demo project is organized into **five parts**:

| Part | Topic | Files |
|------|-------|-------|
| **Part 1** | Official Aspire Integrations | `1-OfficialIntegrations.cs` |
| **Part 2** | Multi-Language App Support | `2-MultiLanguage.cs` |
| **Part 3** | Custom Integration Creation | `3-ItTools.cs`, `4-MailPit.cs` |
| **Part 5** | Advanced Integration Patterns | `5-AdvancedIntegrations.cs` |
| **Part 6** | Fun Demos | `6-Fun.cs` |

## Project Structure

```
AspireAllTheThings/
├── AspireAllTheThings.AppHost/
│   ├── AppHost.cs                    ← Main entry point (uncomment demos here)
│   ├── 1-OfficialIntegrations.cs     ← Part 1: Redis, Postgres, SQL, Azure
│   ├── 2-MultiLanguage.cs            ← Part 2: .NET, Python, Node.js
│   ├── 3-ItTools.cs                  ← Part 3: Simple Docker container
│   ├── 4-MailPit.cs                  ← Part 3: Community Toolkit
│   ├── 5-AdvancedIntegrations.cs     ← Part 5: Discord Notifier (eventing)
│   └── 6-Fun.cs                      ← Part 6: Minecraft
├── AspireAllTheThings.WebApi/        ← Sample ASP.NET Core API
├── python-api/                       ← Sample Python Flask API
└── node-api/                         ← Sample Node.js Express API
```

---

# Part 1: Official Aspire Integrations

These integrations are published and maintained by the .NET Aspire team. They provide first-class support for databases, caches, and Azure services.

## Redis Cache

```csharp
var redis = builder.AddRedis("cache")
    .WithRedisInsight();  // Adds Redis Insight UI for debugging
```

## PostgreSQL

```csharp
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()  // Adds pgAdmin UI for database management
    .AddDatabase("catalogdb");
```

## SQL Server

```csharp
var sqlserver = builder.AddSqlServer("sql")
    .AddDatabase("ordersdb");
```

## Azure Service Bus (with Emulator)

```csharp
var serviceBus = builder.AddAzureServiceBus("messaging")
    .RunAsEmulator();  // Use emulator for local dev
serviceBus.AddServiceBusQueue("orders");
serviceBus.AddServiceBusTopic("events", "audit");
```

## Azure Cosmos DB (with Emulator)

```csharp
var cosmos = builder.AddAzureCosmosDB("cosmos")
    .RunAsEmulator()
    .AddCosmosDatabase("appdata");
```

## Azure Storage (Blobs, Queues, Tables)

```csharp
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();  // Uses Azurite emulator
storage.AddBlobs("blobs");
storage.AddQueues("queues");
storage.AddTables("tables");
```

---

# Part 2: Multi-Language Application Support

Aspire isn't just for .NET! It can orchestrate applications written in any language.

## ASP.NET Core Web API

```csharp
builder.AddProject<Projects.AspireAllTheThings_WebApi>("webapi")
    .WithExternalHttpEndpoints();
```

## Python Flask API

```csharp
builder.AddPythonApp("python-api", "../python-api", "app.py")
    .WithHttpEndpoint(targetPort: 5000, name: "http")
    .WithExternalHttpEndpoints();
```

**Setup:** `cd python-api && pip install -r requirements.txt`

## Node.js Express API

```csharp
builder.AddNpmApp("node-api", "../node-api", "start")
    .WithHttpEndpoint(port: 3000, env: "PORT")
    .WithExternalHttpEndpoints();
```

**Setup:** `cd node-api && npm install`

---

# Part 3: Custom Integration Creation

Build your own integrations with Docker containers!

## Demo: IT-Tools (Simple Docker Container)

**Image:** `corentinth/it-tools`

The simplest way to add ANY Docker container to Aspire.

```csharp
var itTools = builder.AddContainer("it-tools", "corentinth/it-tools")
    .WithHttpEndpoint(targetPort: 80, name: "http")
    .WithExternalHttpEndpoints();
```

**Key Concepts:**
- `AddContainer()` - Add ANY Docker image
- `WithHttpEndpoint()` - Expose HTTP ports
- `WithExternalHttpEndpoints()` - Make accessible from the dashboard

## Demo: MailPit (Community Toolkit)

**Package:** `CommunityToolkit.Aspire.Hosting.Mailpit`

One line of code instead of manual container configuration!

```csharp
var mailpit = builder.AddMailPit("mailpit");
```

**Learn More:** [Aspire Community Toolkit](https://github.com/CommunityToolkit/Aspire)

# Part 6: Fun Demos

Because Aspire isn't just for web apps and databases!

## Demo: Minecraft Server 🎮

**Image:** `itzg/minecraft-server`

Shows environment configuration, volumes, and non-HTTP endpoints.

```csharp
var minecraft = builder.AddContainer("minecraft", "itzg/minecraft-server")
    .WithEnvironment("EULA", "TRUE")
    .WithEnvironment("MODE", "creative")
    .WithEnvironment("MOTD", "Swetugg Stockholm 2026 - Aspire All The Things!")
    .WithEndpoint(targetPort: 25565, port: 25565, name: "minecraft", scheme: "tcp")
    .WithVolume("minecraft-data", "/data")
    .ExcludeFromManifest();  // Dev-only, don't publish
```

**Connect:** `localhost:25565`

---

# Part 5: Advanced Integration Patterns

Learn how to build CUSTOM Aspire integrations with proper eventing patterns.

> ⚠️ **Important Framing:** This is DEV-TIME tooling! The AppHost (and its eventing) doesn't run in production. In production, you'd use Azure Monitor, Prometheus, etc. But the patterns you learn here ARE production-relevant for database seeding, migrations, and integration testing.

## Demo: Discord Notifier 🔔

A custom integration that posts to Discord whenever resources change state.

### Setup

1. Create a Discord webhook: **Server Settings → Integrations → Webhooks → New Webhook**
2. Store the URL in user secrets:

```bash
cd AspireAllTheThings.AppHost
dotnet user-secrets set "Discord:WebhookUrl" "https://discord.com/api/webhooks/YOUR_ID/YOUR_TOKEN"
```

### Usage

```csharp
builder.AddDiscordNotifier("discord-alerts", webhookUrl)
    .NotifyOnStartup()      // 🚀 "Aspire is starting up!"
    .NotifyOnShutdown()     // 👋 "Aspire is shutting down!"
    .WatchAllResources();   // ✅ "cache is ready!" for each resource
```

### Key Concepts Demonstrated

| Concept | What It Shows |
|---------|---------------|
| **Custom Resource Type** | `DiscordNotifierResource` - non-container resource |
| **BeforeStartEvent** | Global event before any resources start |
| **ResourceReadyEvent** | Per-resource event when healthy |
| **ResourceStoppedEvent** | Per-resource event when stopped |
| **ExcludeFromManifest()** | Dev-only resource, won't deploy |
| **Builder Pattern** | `Add*` and `With*` extension methods |

### Live Demo Flow

| Step | What Happens | Discord Shows |
|------|--------------|---------------|
| 1 | Run `aspire run` | 🚀 "Aspire is starting up! Launching X resources..." |
| 2 | Resources start | ✅ "**cache** is ready!" (for each) |
| 3 | Stop Redis in dashboard | 🛑 "**cache** stopped!" |
| 4 | Restart Redis | ✅ "**cache** is ready!" |
| 5 | Ctrl+C | 👋 "Aspire is shutting down!" |

---

## Running the Demos

### Prerequisites
- .NET 10 SDK
- Docker Desktop (or compatible container runtime)
- Aspire workload installed
- Python (for Part 2)
- Node.js (for Part 2)

### Run the AppHost

```bash
cd AspireAllTheThings.AppHost
dotnet run
```

### Enable Demos

Edit `AppHost.cs` and uncomment the demos you want to run:

```csharp
// ---- PART 1: Official Integrations ----
builder.AddRedisDemo();
builder.AddPostgresDemo();

// ---- PART 2: Multi-Language Apps ----
builder.AddAspNetApiDemo();

// ---- PART 3: Custom Integrations ----
builder.AddItToolsDemo();
builder.AddMailPitDemo();

// ---- PART 5: Advanced Integration Patterns ----
builder.AddDiscordNotifierDemo();  // Requires Discord:WebhookUrl in user secrets

// ---- PART 6: Fun Demos ----
builder.AddMinecraftDemo();
```

---

## Key Takeaways

1. **Aspire simplifies distributed development** - No more complex Docker Compose files
2. **Official integrations** - First-class support for databases and Azure services
3. **Multi-language support** - .NET, Python, Node.js, and more
4. **Add ANY container** - If it runs in Docker, it runs in Aspire
5. **Community Toolkit** - Don't reinvent the wheel, leverage community integrations
6. **Beyond web apps** - Aspire is a general-purpose orchestrator

---

## Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire)
- [Aspire Community Toolkit](https://github.com/CommunityToolkit/Aspire)
- [IT-Tools](https://github.com/CorentinTh/it-tools)
- [MailPit](https://github.com/axllent/mailpit)
- [Minecraft Server Docker Image](https://github.com/itzg/docker-minecraft-server)
- [Discord Webhooks Documentation](https://discord.com/developers/docs/resources/webhook)

---

**Speaker:** Jeffrey T. Fritz

🎉 Happy coding at Swetugg Stockholm 2026!
