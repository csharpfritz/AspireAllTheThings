# Part 6: AI Integration with GitHub Models — Demo Script

> **Presenter:** Jeffrey T. Fritz
> **Event:** CodeStock 2026
> **Duration:** ~8–10 minutes
> **File:** `AspireAllTheThings.AppHost/6-AI.cs`

---

## 🔧 Pre-Demo Setup (Before You Go On Stage)

### 1. Get a GitHub Personal Access Token

1. Go to [github.com/settings/tokens](https://github.com/settings/tokens)
2. Create a **fine-grained** token with the **models: read** permission
3. Copy the token (starts with `github_pat_...`)

### 2. Choose Your API Key Strategy

You have **two options** — pick whichever suits your demo style:

**Option A — Pre-configure via user secrets (no prompt on stage):**

```bash
cd AspireAllTheThings.AppHost
dotnet user-secrets set "Parameters:githubApiKey" "github_pat_YOUR_TOKEN"
```

**Option B — Leave it unconfigured so the dashboard prompts you live (recommended for the demo — it shows off the feature!):**

Skip user secrets entirely. The Aspire dashboard will show an interactive parameter prompt at startup because the code uses `AddParameter("githubApiKey", secret: true)`.

### 3. Prepare AppHost.cs

- Comment out **all** Part 1–5 and Part 7 demos (or at minimum, ensure Part 2's `builder.AddAspNetApiDemo()` is commented out — it would conflict with Part 6's WebApi registration)
- **Leave Part 6 commented out** — you'll uncomment it live on stage:

```csharp
// // ---- PART 6: AI Integration (6-AI.cs) ----
// // GitHub Models - AI chat with GenAI dashboard visualizer
// // Set "githubApiKey" parameter in user secrets or dashboard prompt
// builder.AddGitHubModelDemo();
```

### 4. Pre-flight Check

- Docker Desktop is running
- `dotnet build AspireAllTheThings.sln` — verify clean build
- Have a browser tab ready for the Aspire dashboard (usually `https://localhost:15xxx`)
- Have a second browser tab ready for testing the `/chat` endpoint
- Consider having VS Code or Visual Studio open to `6-AI.cs` and `Program.cs` in the WebApi

---

## 🎬 Live Demo Walkthrough

### Step 1: Set the Stage — Show 6-AI.cs

💻 **Open `AspireAllTheThings.AppHost/6-AI.cs` in the IDE.**

🎤 *"Let's talk about AI. Everybody wants to add AI to their app, but how much code does it actually take? Let me show you."*

💻 **Highlight the entire `AddGitHubModelDemo` method (lines 39–54):**

```csharp
public static IDistributedApplicationBuilder AddGitHubModelDemo(this IDistributedApplicationBuilder builder)
{
    // Secret parameter — the Aspire dashboard will prompt for this interactively
    var apiKey = builder.AddParameter("githubApiKey", secret: true);

    // Register a GitHub Model resource using GPT-4o-mini
    var chat = builder.AddGitHubModel("chat", "openai/gpt-4o-mini")
        .WithApiKey(apiKey);

    // Wire the model to the WebApi project so it can use IChatClient
    builder.AddProject<Projects.AspireAllTheThings_WebApi>("webapi")
        .WithExternalHttpEndpoints()
        .WithReference(chat);

    return builder;
}
```

🎤 **Talking points (work through line by line):**

1. *"This entire AI integration is **15 lines of code** including comments. That's it."*
2. *"First, we declare a secret parameter. The `secret: true` flag is key — Aspire's dashboard will actually **prompt you** for this value interactively at startup. No more pasting secrets into config files and hoping you don't commit them."*
3. *"Next, `AddGitHubModel` — this registers a GitHub Models resource. We're using GPT-4o-mini here, but you could swap in any model GitHub Models supports. You wire up the API key with `.WithApiKey()`."*
4. *"Finally, we add our WebApi project and give it a `.WithReference(chat)` — that's it. Aspire handles the service discovery, connection strings, and OpenTelemetry instrumentation automatically."*

---

### Step 2: Show the Client Side — WebApi/Program.cs

💻 **Switch to `AspireAllTheThings.WebApi/Program.cs`. Highlight the AI registration (lines 12–16):**

```csharp
// Register the AI chat client from GitHub Models when Part 6 is enabled
if (builder.Configuration.GetConnectionString("chat") is not null)
{
    builder.AddAzureChatCompletionsClient("chat")
        .AddChatClient();
}
```

🎤 *"On the client side — in the WebApi — we check if the `chat` connection is available. If it is, we register the AI client. `AddAzureChatCompletionsClient` comes from the Aspire client package, and `.AddChatClient()` registers `IChatClient` in DI."*

🎤 *"Notice this uses `IChatClient` from `Microsoft.Extensions.AI` — that's the provider-agnostic abstraction. Today we're using GitHub Models with GPT-4o-mini, but you could swap to Azure OpenAI, Ollama, or any other provider without changing your endpoint code."*

💻 **Scroll down to the `/chat` endpoint (around line 50):**

```csharp
app.MapPost("/chat", async (IChatClient? chatClient, ChatRequest request) =>
{
    if (chatClient is null)
        return Results.Problem("AI not configured. Enable Part 6 in AppHost.cs and set the GitHub API key.");

    var prompt = request.Message ?? "Hello! What can you help me with today?";
    var response = await chatClient.GetResponseAsync(prompt);
    return Results.Ok(new { prompt, response = response.Text });
})
.WithName("Chat");

record ChatRequest(string? Message);
```

🎤 *"And here's the endpoint. Minimal API, accepts a JSON body with the message, injects `IChatClient`, calls `GetResponseAsync`, done. The nullable `IChatClient?` is a nice pattern — if Part 6 isn't enabled, the endpoint gracefully tells you rather than crashing."*

---

### Step 3: Uncomment in AppHost.cs & Run

💻 **Switch to `AppHost.cs`. Uncomment the Part 6 line:**

```csharp
// ---- PART 6: AI Integration (6-AI.cs) ----
// GitHub Models - AI chat with GenAI dashboard visualizer
// Set "githubApiKey" parameter in user secrets or dashboard prompt
builder.AddGitHubModelDemo();
```

🎤 *"Now let's light it up. I uncomment `builder.AddGitHubModelDemo()` — that's one line — and run."*

💻 **Run the AppHost:**

```bash
cd AspireAllTheThings.AppHost
dotnet run
```

---

### Step 4: The Parameter Prompt (if using Option B)

💻 **Open the Aspire dashboard in the browser.**

🎤 *"Watch what happens — the dashboard is showing me a parameter prompt. It's asking for my GitHub API key. Remember that `secret: true` flag? This is what it does."*

💻 **Paste your GitHub PAT into the prompt and submit.**

🎤 *"I paste in my token, hit submit, and the resources start up. No config files, no environment variables, no user secrets needed — the dashboard handles it interactively. This is great for demos, for onboarding new team members, or for any secret you don't want living in a file."*

> **If using Option A (pre-configured user secrets):** Skip the prompt demo and instead say: *"I pre-configured my API key in user secrets, but if I hadn't, the dashboard would have prompted me for it — that's the `secret: true` parameter feature."*

---

### Step 5: Show the Dashboard — Resources & GenAI Visualizer

💻 **In the Aspire dashboard, show the resource list.**

🎤 *"In the dashboard you can see our `chat` resource — that's the GitHub Model — and our `webapi` project. Everything is wired up and healthy."*

💻 **Navigate to the Structured Logs or Traces view — look for the GenAI visualizer.**

🎤 *"Now let me make a request so we can see the GenAI visualizer in action..."*

---

### Step 6: Test the Chat UI Live

💻 **In the Aspire dashboard, click the webapi endpoint URL to open the chat UI. Navigate to `/chat.html`:**

```
https://localhost:{port}/chat.html
```

🎤 *"Now let's see this in action. We have a chat UI — it's a single HTML file with inline CSS and JavaScript, served as a static file from the WebApi. Clean, modern, dark theme to match the Aspire dashboard vibes."*

💻 **Type a question in the chat UI:**

```
What is .NET Aspire in one sentence?
```

🎤 *"Let's ask it a question... 'What is .NET Aspire in one sentence?'"*

💻 **Hit Send and show the response appearing in the chat.**

🎤 *"There it is — a response from GPT-4o-mini, through GitHub Models, orchestrated by Aspire. The chat UI shows the conversation history, has a nice loading indicator, and handles errors gracefully. Let's look at what the dashboard captured."*

---

### Step 7: Show the GenAI Visualizer

💻 **Go back to the Aspire dashboard. Navigate to the Traces view and find the AI inference trace.**

🎤 *"Here's the magic — the **GenAI visualizer**. Aspire automatically captures AI telemetry through OpenTelemetry. No extra configuration, no custom middleware. You can see:"*

- *"**Token usage** — how many tokens the prompt and completion used"*
- *"**Latency** — how long the inference took"*
- *"**The prompt and completion** — what you sent and what you got back"*
- *"**The model** — which model handled the request"*

🎤 *"This is automatic AI observability. In production, this same telemetry flows to Azure Monitor, Seq, Jaeger — wherever your OpenTelemetry goes. You get AI cost tracking and debugging for free."*

---

### Step 8: Wrap Up the Demo

🎤 *"So let's recap what we just did:*

1. ***15 lines in the AppHost** to register a GitHub Model and wire it to our API*
2. ***4 lines in the WebApi** to register the AI client*
3. ***A simple endpoint** that uses `IChatClient` — provider-agnostic, swappable*
4. ***Automatic AI observability** in the dashboard with zero extra code*
5. ***Interactive secret prompting** — no config file secrets to manage*

*That's AI integration with Aspire. It's boring in the best possible way — it just works."*

---

## 🗣️ Key Talking Points Summary

| Topic | What to Emphasize |
|-------|-------------------|
| **Developer Productivity** | 15 lines of AppHost code, 4 lines on the client. AI integration shouldn't be hard. |
| **Secret Parameter Prompts** | `secret: true` triggers dashboard prompts — great for API keys, no files to manage |
| **GenAI Visualizer** | Automatic AI observability via OpenTelemetry — token usage, latency, prompts/completions |
| **IChatClient Abstraction** | `Microsoft.Extensions.AI` — swap GitHub Models for Azure OpenAI, Ollama, etc. without changing endpoints |
| **GitHub Models** | Free tier available, great for dev/demos, access to GPT-4o-mini and other models |
| **NuGet Packages** | AppHost: `Aspire.Hosting.GitHub.Models` (13.1.0). Client: `Aspire.Azure.AI.Inference` (preview) |

---

## ❓ Potential Audience Questions & Answers

### "Can I use Azure OpenAI instead of GitHub Models?"

> *"Absolutely. The `IChatClient` interface is from Microsoft.Extensions.AI — it's provider-agnostic. On the AppHost side you'd swap `AddGitHubModel` for `AddAzureOpenAI`, and on the client side you'd change the client registration. The `/chat` endpoint code stays exactly the same."*

### "Is GitHub Models free?"

> *"GitHub Models has a free tier that's great for development and demos. For production workloads, you'd typically use Azure OpenAI or another provider. The beauty of the `IChatClient` abstraction is that switching providers is a config change, not a code rewrite."*

### "What about the GenAI telemetry — does that work in production?"

> *"Yes! The telemetry flows through OpenTelemetry, which is the same standard you'd use in production. The Aspire dashboard is a dev-time tool, but in production that same telemetry goes to Azure Monitor, Grafana, Datadog — wherever your OTLP exporter points."*

### "What models are available through GitHub Models?"

> *"GitHub Models gives you access to a catalog of models including GPT-4o, GPT-4o-mini, Phi, Llama, Mistral, and others. You just change the model string in `AddGitHubModel` — for example, `\"openai/gpt-4o\"` or `\"meta/llama-3-8b-instruct\"`."*

### "Does the secret parameter prompt work in CI/CD?"

> *"In CI/CD you'd use environment variables or user secrets — the interactive prompt is a dev-time feature. Aspire parameters fall back to configuration in the standard .NET order: environment variables, user secrets, appsettings.json. The prompt is just the dashboard's UX for when no value is found."*

### "What's the `Aspire.Azure.AI.Inference` package? Why 'Azure' if we're using GitHub?"

> *"GitHub Models uses the Azure AI Inference SDK under the hood — GitHub's model API is compatible with the Azure AI Inference protocol. So the client package is `Aspire.Azure.AI.Inference` even though the hosting resource is `Aspire.Hosting.GitHub.Models`. The `.AddChatClient()` extension bridges it to the `IChatClient` abstraction."*

### "Can I use this with local models like Ollama?"

> *"Yes! Aspire has an `Aspire.Hosting.Ollama` community integration. You'd use `AddOllama` on the AppHost side and a different client registration, but the endpoint code using `IChatClient` would remain identical. That's the power of the abstraction."*

---

## 📝 Stage Notes

- **Timing tip:** The GitHub Models API call takes 1–3 seconds. Fill the wait with the GenAI visualizer explanation.
- **Backup plan:** If GitHub Models is down or slow, have a screenshot of the GenAI visualizer ready, and explain the flow conceptually.
- **Watch out:** Don't uncomment Part 2's `builder.AddAspNetApiDemo()` at the same time as Part 6 — both register the `webapi` resource and will conflict.
- **Minor note:** The comments in `WebApi/Program.cs` (lines 11 and 48) reference "Part 7" instead of "Part 6" — this is a cosmetic issue from the reorder. Ignore it during the demo; the code works correctly.
- **Network dependency:** This demo requires internet access to reach `https://models.inference.ai.azure.com`. Confirm venue Wi-Fi before going on stage.
