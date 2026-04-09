# Part 5: Advanced Integration Patterns — Discord Notifier Demo Script

> **Presenter:** Jeffrey T. Fritz
> **Event:** CodeStock 2026
> **Duration:** ~10–12 minutes
> **File:** `AspireAllTheThings.AppHost/5-AdvancedIntegrations.cs`

---

## 🔧 Pre-Demo Setup (Before You Go On Stage)

### 1. Create a Discord Webhook

1. Open Discord → go to your demo server
2. **Server Settings → Integrations → Webhooks → New Webhook**
3. Copy the webhook URL (format: `https://discord.com/api/webhooks/{id}/{token}`)
4. Choose a channel name to use (e.g. `aspire-demo`)

### 2. Choose Your Configuration Strategy

You have **two options** — pick whichever suits your demo style:

**Option A — Pre-configure via user secrets (no prompt on stage):**

```bash
cd AspireAllTheThings.AppHost
dotnet user-secrets set "Parameters:discordWebhookUrl" "https://discord.com/api/webhooks/YOUR_ID/YOUR_TOKEN"
dotnet user-secrets set "Parameters:discordChannel" "aspire-demo"
```

**Option B — Leave unconfigured so the dashboard prompts you live (recommended — it shows off the parameter prompt feature!):**

Skip user secrets entirely. The Aspire dashboard will display interactive parameter prompts at startup because the code uses `AddParameter("discordWebhookUrl", secret: true)` and `AddParameter("discordChannel")`.

### 3. Prepare AppHost.cs

- Comment out **all** other Part demos (or at minimum avoid Part 2 and Part 6 which register `webapi` — they'll conflict if both are active)
- **Leave Part 5 commented out** — you'll uncomment it live:

```csharp
// // ---- PART 5: Advanced Integration Patterns (5-AdvancedIntegrations.cs) ----
// // Discord Notifier - eventing, custom resources, parameter prompts
// // Dashboard prompts for webhook URL + channel
// builder.AddDiscordNotifierDemo();
```

### 4. Pre-flight Check

- Docker Desktop is running (needed if other parts add containers)
- `dotnet build AspireAllTheThings.sln` — verify clean build
- Have a browser tab ready for the Aspire dashboard
- Have your Discord channel visible on screen (split-screen recommended)
- Have your webhook URL in a text file ready to paste
- Consider having VS Code or Visual Studio open to `5-AdvancedIntegrations.cs`

---

## 🎬 Live Demo Walkthrough

### Step 1: Set the Stage — Explain Custom Integrations

🎤 *"So far we've used official Aspire integrations and Docker containers. But what if you want to build something entirely custom — something that isn't a container or a project at all? Let me show you a custom Discord notifier that watches your resources and posts to a channel when they change state."*

🎤 *"And along the way, we'll see a really cool Aspire feature: interactive parameter prompts in the dashboard."*

---

### Step 2: Show the Parameter Prompts — AddParameter + WithDescription

💻 **Open `5-AdvancedIntegrations.cs`. Highlight the `AddDiscordNotifierDemo` method (lines 48–73):**

```csharp
public static IDistributedApplicationBuilder AddDiscordNotifierDemo(
    this IDistributedApplicationBuilder builder)
{
    builder.AddParameter("discordWebhookUrl", secret: true)
        .WithDescription(
            "**Discord Webhook URL**\n\n" +
            "Create a webhook in Discord: _Server Settings → Integrations → Webhooks → New Webhook_\n\n" +
            "Format: `https://discord.com/api/webhooks/{id}/{token}`",
            enableMarkdown: true);

    builder.AddParameter("discordChannel")
        .WithDescription(
            "**Discord Channel Name**\n\n" +
            "The channel name to display in notification messages (e.g. `aspire-demo`, `general`).",
            enableMarkdown: true);

    builder.AddDiscordNotifier("discord-alerts")
        .NotifyOnStartup()
        .NotifyOnShutdown()
        .WatchAllResources();

    return builder;
}
```

🎤 **Talking points (work through the parameters):**

1. *"Notice `AddParameter("discordWebhookUrl", secret: true)` — the `secret: true` flag tells Aspire this is sensitive. The dashboard will show a **masked password field** for it."*
2. *"Then there's `AddParameter("discordChannel")` — no `secret` flag, so the dashboard shows a regular text field."*
3. *"Both use `.WithDescription()` with `enableMarkdown: true`. That means the prompt dialog in the dashboard renders **rich Markdown** — bold text, code blocks, even italics. This is great for giving your team helpful setup instructions right in the UI."*
4. *"You can pre-fill these via user secrets (`Parameters:discordWebhookUrl` and `Parameters:discordChannel`), or just leave them blank and the dashboard will ask for them interactively."*

---

### Step 3: Show the Custom Resource Type

💻 **Scroll down to the `DiscordNotifierResource` class (around line 97):**

```csharp
public sealed class DiscordNotifierResource : Resource, IResourceWithWaitSupport
{
    private readonly IConfiguration _configuration;

    public string WebhookUrl => _configuration["Parameters:discordWebhookUrl"] ?? "";
    public string ChannelName => _configuration["Parameters:discordChannel"] ?? "general";
    // ...
}
```

🎤 *"Here's the custom resource. Notice it extends `Resource` — it's not a container, not a project. It's a logical resource that appears in the dashboard but doesn't run anything itself."*

🎤 *"The webhook URL and channel name are read lazily from `IConfiguration`. That's important — when the user enters values in the dashboard prompt, they get written into configuration, and we read them at event time. No eagerly captured values that miss the prompt input."*

---

### Step 4: Show the Eventing Pattern

💻 **Highlight the eventing methods — `NotifyOnStartup`, `WatchAllResources`:**

🎤 *"Aspire has a proper eventing system. `BeforeStartEvent` fires once before anything starts — great for initialization. `ResourceReadyEvent` fires per resource when it's healthy. `ResourceStoppedEvent` when something goes down."*

🎤 *"The notification messages include the channel name from our parameter — so you see exactly where alerts are going."*

🎤 *"And remember: this is dev-time tooling. In production, you wouldn't use the AppHost. But these same patterns — subscribing to resource lifecycle events — are exactly how you'd do database seeding, run migrations, or set up integration tests."*

---

### Step 5: Uncomment in AppHost.cs & Run

💻 **Switch to `AppHost.cs`. Uncomment the Part 5 line:**

```csharp
// ---- PART 5: Advanced Integration Patterns (5-AdvancedIntegrations.cs) ----
// Discord Notifier - eventing, custom resources, parameter prompts
// Dashboard prompts for webhook URL + channel
builder.AddDiscordNotifierDemo();
```

🎤 *"One line. Let's run it and see what happens."*

💻 **Run the AppHost:**

```bash
cd AspireAllTheThings.AppHost
dotnet run
```

---

### Step 6: The Parameter Prompts in the Dashboard (The Money Shot)

💻 **Open the Aspire dashboard in the browser. The parameter prompt dialog should appear.**

🎤 *"Look at this — the dashboard is prompting me for two values before it starts. First, the webhook URL — see how it's a **masked password field**? That's the `secret: true` flag in action. And below it, the channel name — a regular text field."*

💻 **Click the info/description icon next to the webhook URL field.**

🎤 *"And check this out — the description text we wrote with `.WithDescription()` is rendered as **Markdown** right in the dialog. Bold headings, code formatting, italics — all rendered beautifully. This is how you give your team helpful onboarding instructions without a separate wiki page."*

💻 **Paste your webhook URL into the secret field. Type your channel name (e.g. `aspire-demo`). Submit.**

🎤 *"I paste in my webhook URL, type the channel name, and submit. The resources start spinning up."*

> **If using Option A (pre-configured secrets):** Skip the prompt demo and instead say: *"I pre-configured these values in user secrets, but if I hadn't, the dashboard would have prompted me — one masked field for the secret URL, one text field for the channel name, both with Markdown help text."*

---

### Step 7: Watch Discord Light Up

💻 **Switch to your Discord channel. Messages should start appearing:**

🎤 *"And there it is — Discord just got a notification: 'Aspire is starting up! Launching N resources...' And notice it says 'Posting to #aspire-demo' — that's our channel parameter in action."*

💻 **Wait for resources to come up. Show the "resource is ready" messages rolling in:**

🎤 *"Now watch — as each resource becomes healthy, we get a notification. 'Cache is ready!' 'postgres is ready!' Each one with a nice emoji and a timestamp."*

---

### Step 8: Demonstrate Resource Lifecycle Events

💻 **In the Aspire dashboard, stop a resource (e.g. Redis) using the Stop button.**

🎤 *"Let me stop Redis from the dashboard..."*

💻 **Show the Discord "stopped" notification.**

🎤 *"There — Discord says 'cache stopped!' with a red embed. Now let me restart it..."*

💻 **Restart the resource.**

🎤 *"And there's the 'cache is ready!' message again. Real-time lifecycle notifications."*

---

### Step 9: Wrap Up

🎤 *"So let's recap what we just built:*

1. ***Custom resource type** — not a container, not a project, just a logical component in the Aspire model*
2. ***Interactive parameter prompts** — the dashboard asked for our webhook URL (secret, masked) and channel name (regular text field)*
3. ***Rich Markdown descriptions** — `.WithDescription()` renders formatted help text right in the prompt UI*
4. ***Aspire eventing** — `BeforeStartEvent`, `ResourceReadyEvent`, `ResourceStoppedEvent` for lifecycle hooks*
5. ***Dev-time tooling** with production-relevant patterns — these same event patterns power database seeding and migration scenarios*

*This is the power of custom Aspire integrations. You're not limited to what the framework ships — you can build anything."*

---

## 🗣️ Key Talking Points Summary

| Topic | What to Emphasize |
|-------|-------------------|
| **Parameter Prompts** | `AddParameter(secret: true)` for masked fields, `AddParameter()` for regular text — dashboard prompts at startup |
| **WithDescription + Markdown** | `.WithDescription(text, enableMarkdown: true)` renders rich help text in the prompt dialog |
| **Two Config Paths** | Pre-fill via `dotnet user-secrets set "Parameters:..."` OR enter interactively in the dashboard |
| **Custom Resources** | Resources can be logical components — no container required. Extend `Resource` base class |
| **Eventing System** | `BeforeStartEvent` (global), `ResourceReadyEvent` / `ResourceStoppedEvent` (per-resource) |
| **Dev-Time Tooling** | AppHost doesn't run in production, but patterns transfer to seeding, migrations, integration tests |
| **Lazy Configuration** | Read from `IConfiguration` at event time, not at registration — ensures dashboard prompt values are available |

---

## ❓ Potential Audience Questions & Answers

### "Can I use parameter prompts for any configuration value?"

> *"Yes! `AddParameter()` works for any string value your resources need. Use `secret: true` for sensitive values like API keys, connection strings, or tokens — the dashboard masks the input. Without `secret`, it's a regular text field. Both support `.WithDescription()` for help text."*

### "What if I pre-configure the values in user secrets?"

> *"Aspire parameters follow the standard .NET configuration order — environment variables, user secrets, appsettings.json. If a value is already set, the dashboard won't prompt for it. The prompt only appears when no value is found. So you can pre-fill for CI/CD and still get prompts for local dev."*

### "Does `.WithDescription()` support full Markdown?"

> *"It supports a subset of Markdown that's useful for help text — bold, italic, code blocks, inline code, and links. The `enableMarkdown: true` flag turns on the rendering. Without it, the description is shown as plain text."*

### "Could I use this eventing pattern for database migrations?"

> *"Absolutely — that's one of the best real-world uses. Subscribe to `ResourceReadyEvent` on your database resource, and run migrations when it's healthy. Same pattern as we used for Discord, but instead of posting a webhook, you'd run `dotnet ef database update` or your migration tool of choice."*

### "Does the Discord notifier run in production?"

> *"No — and that's intentional. The AppHost is dev-time tooling. It doesn't appear in deployment manifests (we call `ExcludeFromManifest()`). In production, you'd use Azure Monitor alerts, PagerDuty, or Prometheus for this kind of notification. But the builder pattern and eventing concepts absolutely carry over."*

### "What's `IResourceWithWaitSupport`?"

> *"It's an interface that says 'other resources can call `.WaitFor()` on me.' Without it, Aspire can't health-check or wait for your resource. Our notifier implements it so downstream resources could theoretically wait for the notifier to be ready before starting."*

---

## 📝 Stage Notes

- **Split screen recommended:** Have the Aspire dashboard on one side and Discord on the other for maximum visual impact.
- **Timing tip:** Resource ready messages come in fast once things spin up. Narrate them as they appear.
- **Backup plan:** If Discord webhook fails (rate limit, network), the console logs will show `⚠️ Failed to post to Discord: ...` — explain that the code handles failures gracefully without crashing.
- **Parameter prompt tip:** Click the description/info icon in the prompt dialog to show the Markdown rendering — this is a great visual moment.
- **Enable Part 1 alongside Part 5:** Having Redis and Postgres running gives you resources to watch and stop/restart for the lifecycle demo. Don't enable Parts 2 or 6 (webapi conflict).
- **Key transition:** After this demo, segue to Part 6 (AI) with: *"We've seen parameters for secrets — now let's use one for an AI API key and see the GenAI visualizer."*
