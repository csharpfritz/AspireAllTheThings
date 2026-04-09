using System.Net.Http.Json;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Eventing;
using Microsoft.Extensions.Configuration;

namespace AspireAllTheThings.AppHost;

/// <summary>
/// PART 5: Advanced Integration Patterns - Discord Notifier
/// 
/// This demo teaches you how to build CUSTOM Aspire integrations by creating
/// a Discord notifier that posts to a channel whenever resources change state.
/// 
/// 🎯 IMPORTANT FRAMING:
/// This is DEV-TIME tooling! The AppHost (and its eventing) doesn't run in production.
/// In production, you'd use Azure Monitor, Prometheus, etc. for alerting.
/// 
/// BUT the patterns you learn here ARE production-relevant:
/// - Database seeding when a DB is ready
/// - Running migrations before apps start  
/// - Custom initialization logic
/// - Integration testing with DistributedApplicationTestingBuilder
/// 
/// Key Concepts Demonstrated:
/// 1. Custom Resource Types (non-container resources!)
/// 2. Global vs Resource-Specific Eventing
/// 3. BeforeStartEvent, ResourceReadyEvent, ResourceStoppedEvent
/// 4. Extension method patterns (Add*, With*)
/// 5. IResourceWithWaitSupport for WaitFor() compatibility
/// 6. Interactive Parameter Prompts — the dashboard prompts for values at startup!
/// </summary>
public static class AdvancedIntegrationsDemo
{
    /// <summary>
    /// Adds the Discord Notifier demo with interactive parameter prompts.
    /// 
    /// This demo showcases Aspire's parameter prompt feature in the dashboard:
    /// - Secret parameters (webhook URL) show a masked password field
    /// - Regular parameters (channel name) show a standard text field
    /// 
    /// SETUP OPTIONS:
    /// Option A — Pre-fill via user secrets (no prompt):
    ///   dotnet user-secrets set "Parameters:discordWebhookUrl" "https://discord.com/api/webhooks/..."
    ///   dotnet user-secrets set "Parameters:discordChannel" "aspire-demo"
    /// 
    /// Option B — Leave blank and enter values in the Aspire dashboard prompt at startup
    /// </summary>
    public static IDistributedApplicationBuilder AddDiscordNotifierDemo(
        this IDistributedApplicationBuilder builder)
    {
        // Interactive parameter prompts — these appear in the Aspire dashboard!
        // Secret: masked password field  |  Regular: standard text field
        // WithDescription sets the prompt text shown in the dashboard dialog
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
}

// ============================================================================
// PART 1: CUSTOM RESOURCE TYPE
// ============================================================================
// 
// Resources don't have to be containers! This is a "logical" resource that
// represents a notification channel. It appears in the dashboard but doesn't
// run anything - it just does work when events fire.
// ============================================================================

/// <summary>
/// A custom resource that sends notifications to Discord when other resources
/// change state. This demonstrates that Aspire resources can be more than just
/// containers or executables - they can be any logical component of your system.
/// 
/// Key Interfaces:
/// - Resource: Base class for all Aspire resources
/// - IResourceWithWaitSupport: Allows other resources to use WaitFor() on this
/// 
/// Parameter values (webhook URL, channel name) are read lazily from configuration
/// so that dashboard-prompted values are available when events fire.
/// </summary>
public sealed class DiscordNotifierResource : Resource, IResourceWithWaitSupport
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// The Discord webhook URL, resolved from the "discordWebhookUrl" parameter.
    /// Read lazily so dashboard-prompted values are available at event time.
    /// </summary>
    public string WebhookUrl => _configuration["Parameters:discordWebhookUrl"] ?? "";

    /// <summary>
    /// The Discord channel name to display in notifications.
    /// Resolved from the "discordChannel" parameter.
    /// </summary>
    public string ChannelName => _configuration["Parameters:discordChannel"] ?? "general";

    /// <summary>
    /// Track which resources we're actively watching.
    /// This is just for informational purposes in the dashboard.
    /// </summary>
    internal List<string> WatchedResourceNames { get; } = [];

    /// <summary>
    /// Creates a new Discord notifier resource.
    /// </summary>
    /// <param name="name">The resource name (appears in Aspire dashboard)</param>
    /// <param name="configuration">Configuration instance for lazy parameter resolution</param>
    public DiscordNotifierResource(string name, IConfiguration configuration) : base(name)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
}

// ============================================================================
// PART 2: EXTENSION METHODS (THE BUILDER PATTERN)
// ============================================================================
// 
// Aspire uses the builder pattern extensively. Each extension method:
// - Starts with Add* (to add a resource) or With* (to configure one)
// - Returns IResourceBuilder<T> to enable method chaining
// - Lives in the Aspire.Hosting namespace for discoverability
// ============================================================================

/// <summary>
/// Extension methods for adding and configuring the Discord Notifier.
/// 
/// Following Aspire conventions:
/// - Add* methods: Add a new resource to the app model
/// - With* methods: Configure an existing resource
/// - Namespace: Aspire.Hosting for discoverability via IntelliSense
/// </summary>
public static class DiscordNotifierResourceBuilderExtensions
{
    // ========================================================================
    // ADD METHOD - Creates the resource
    // ========================================================================
    
    /// <summary>
    /// Adds a Discord notifier to the application model.
    /// 
    /// This resource will post messages to a Discord channel when other
    /// resources in the application change state. The webhook URL and channel
    /// name are read from Aspire parameters (dashboard prompts or user secrets).
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the resource (e.g., "discord-alerts").</param>
    /// <returns>A resource builder for further configuration.</returns>
    /// <example>
    /// <code>
    /// builder.AddParameter("discordWebhookUrl", secret: true);
    /// builder.AddParameter("discordChannel");
    /// 
    /// var discord = builder.AddDiscordNotifier("alerts")
    ///     .NotifyOnStartup()
    ///     .WatchAllResources();
    /// </code>
    /// </example>
    public static IResourceBuilder<DiscordNotifierResource> AddDiscordNotifier(
        this IDistributedApplicationBuilder builder,
        string name)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new DiscordNotifierResource(name, builder.Configuration);

        // AddResource is the core Aspire API for adding resources to the app model.
        // It wraps your resource in an IResourceBuilder<T> for further configuration.
        return builder.AddResource(resource)
            .ExcludeFromManifest();  // Don't include in deployment manifest - dev only!
    }

    // ========================================================================
    // GLOBAL EVENTS - Subscribe to application-wide lifecycle events
    // ========================================================================

    /// <summary>
    /// Posts a notification when Aspire starts up.
    /// 
    /// EVENTING PATTERN: BeforeStartEvent
    /// This is a GLOBAL event that fires once before any resources start.
    /// Use it for initialization that needs to happen before the app runs.
    /// </summary>
    public static IResourceBuilder<DiscordNotifierResource> NotifyOnStartup(
        this IResourceBuilder<DiscordNotifierResource> builder)
    {
        var notifier = builder.Resource;
        var eventing = builder.ApplicationBuilder.Eventing;

        // Subscribe to BeforeStartEvent - fires ONCE before resources start
        eventing.Subscribe<BeforeStartEvent>(async (evt, ct) =>
        {
            var resourceCount = evt.Model.Resources.Count();
            
            await PostToDiscordAsync(
                notifier.WebhookUrl,
                title: "🚀 Aspire is starting up!",
                description: $"Launching **{resourceCount}** resources...\n" +
                            $"📢 Posting to **#{notifier.ChannelName}**\n\n" +
                            $"_CodeStock 2026 - Aspire All The Things!_",
                color: DiscordColors.Blue,
                ct);
        });

        return builder;
    }

    /// <summary>
    /// Posts a notification when Aspire shuts down.
    /// 
    /// EVENTING PATTERN: AfterResourcesCreatedEvent
    /// Note: There isn't a perfect "shutdown" event, but we can detect
    /// when the application is stopping via CancellationToken patterns.
    /// </summary>
    public static IResourceBuilder<DiscordNotifierResource> NotifyOnShutdown(
        this IResourceBuilder<DiscordNotifierResource> builder)
    {
        var notifier = builder.Resource;
        var eventing = builder.ApplicationBuilder.Eventing;

        // We'll post when all resources are created and register a shutdown handler
        eventing.Subscribe<AfterResourcesCreatedEvent>(async (evt, ct) =>
        {
            // Register to post when cancellation is requested (app shutting down)
            ct.Register(async () =>
            {
                try
                {
                    await PostToDiscordAsync(
                        notifier.WebhookUrl,
                        title: "👋 Aspire is shutting down!",
                        description: "All resources are being stopped.\n\n" +
                                    "_Thanks for watching the demo!_",
                        color: DiscordColors.Gray,
                        CancellationToken.None);  // Can't use cancelled token!
                }
                catch
                {
                    // Swallow errors during shutdown - we're already dying
                }
            });
        });

        return builder;
    }

    // ========================================================================
    // RESOURCE-SPECIFIC EVENTS - Subscribe to individual resource lifecycle
    // ========================================================================

    /// <summary>
    /// Watches ALL resources and posts notifications when they change state.
    /// 
    /// EVENTING PATTERN: Resource-specific events
    /// - ResourceReadyEvent: Resource started and passed health checks
    /// - ResourceStoppedEvent: Resource was stopped
    /// 
    /// This demonstrates iterating over the app model and subscribing to
    /// events for each resource dynamically.
    /// </summary>
    public static IResourceBuilder<DiscordNotifierResource> WatchAllResources(
        this IResourceBuilder<DiscordNotifierResource> builder)
    {
        var notifier = builder.Resource;
        var eventing = builder.ApplicationBuilder.Eventing;

        // Subscribe to BeforeStartEvent to set up watchers before resources start
        eventing.Subscribe<BeforeStartEvent>(async (evt, ct) =>
        {
            foreach (var resource in evt.Model.Resources)
            {
                // Don't watch ourselves - that would be weird!
                if (resource == notifier) continue;

                // Don't watch resources that don't have meaningful lifecycle
                // (e.g., parameters, connection strings)
                if (resource is not IResourceWithWaitSupport) continue;

                WatchResource(eventing, notifier, resource);
            }

            // Post a summary of what we're watching
            await PostToDiscordAsync(
                notifier.WebhookUrl,
                title: $"👀 Watching resources → #{notifier.ChannelName}",
                description: string.Join("\n", notifier.WatchedResourceNames.Select(n => $"• {n}")),
                color: DiscordColors.Purple,
                ct);
        });

        return builder;
    }

    /// <summary>
    /// Watches a specific resource and posts notifications when it changes state.
    /// 
    /// Use this when you only want to watch high-value resources (databases, APIs)
    /// rather than everything.
    /// </summary>
    public static IResourceBuilder<DiscordNotifierResource> WatchResource<T>(
        this IResourceBuilder<DiscordNotifierResource> builder,
        IResourceBuilder<T> target) where T : IResource
    {
        var notifier = builder.Resource;
        var eventing = builder.ApplicationBuilder.Eventing;

        WatchResource(eventing, notifier, target.Resource);

        return builder;
    }

    /// <summary>
    /// Internal helper to set up event subscriptions for a single resource.
    /// </summary>
    private static void WatchResource(
        IDistributedApplicationEventing eventing,
        DiscordNotifierResource notifier,
        IResource target)
    {
        notifier.WatchedResourceNames.Add(target.Name);

        // ✅ ResourceReadyEvent - Resource is running and healthy
        eventing.Subscribe<ResourceReadyEvent>(target, async (evt, ct) =>
        {
            var emoji = GetResourceEmoji(target);
            
            await PostToDiscordAsync(
                notifier.WebhookUrl,
                title: $"{emoji} **{target.Name}** is ready!",
                description: GetResourceDescription(target),
                color: DiscordColors.Green,
                ct);
        });

        // 🛑 ResourceStoppedEvent - Resource was stopped  
        eventing.Subscribe<ResourceStoppedEvent>(target, async (evt, ct) =>
        {
            await PostToDiscordAsync(
                notifier.WebhookUrl,
                title: $"🛑 **{target.Name}** stopped!",
                description: "Resource has been stopped.",
                color: DiscordColors.Gray,
                ct);
        });
    }

    // ========================================================================
    // DISCORD API HELPERS
    // ========================================================================

    /// <summary>
    /// Posts a rich embed message to Discord via webhook.
    /// 
    /// Discord Webhook API: https://discord.com/developers/docs/resources/webhook
    /// Embed Object: https://discord.com/developers/docs/resources/message#embed-object
    /// </summary>
    private static async Task PostToDiscordAsync(
        string webhookUrl,
        string title,
        string description,
        int color,
        CancellationToken ct)
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);  // Don't hang the app

            var payload = new
            {
                embeds = new[]
                {
                    new
                    {
                        title,
                        description,
                        color,
                        timestamp = DateTime.UtcNow.ToString("o"),
                        footer = new 
                        { 
                            text = "Aspire All The Things! 🚀",
                            icon_url = "https://raw.githubusercontent.com/dotnet/aspire/main/docs/images/aspire-icon-64.png"
                        }
                    }
                }
            };

            await client.PostAsJsonAsync(webhookUrl, payload, ct);
        }
        catch (Exception ex)
        {
            // Log but don't crash - notifications are nice-to-have
            Console.WriteLine($"⚠️  Failed to post to Discord: {ex.Message}");
        }
    }

    /// <summary>
    /// Returns an emoji based on the resource type - makes Discord messages more fun!
    /// </summary>
    private static string GetResourceEmoji(IResource resource) => resource switch
    {
        ContainerResource c when c.Name.Contains("redis", StringComparison.OrdinalIgnoreCase) => "🔴",
        ContainerResource c when c.Name.Contains("postgres", StringComparison.OrdinalIgnoreCase) => "🐘",
        ContainerResource c when c.Name.Contains("sql", StringComparison.OrdinalIgnoreCase) => "🗃️",
        ContainerResource c when c.Name.Contains("mongo", StringComparison.OrdinalIgnoreCase) => "🍃",
        ContainerResource c when c.Name.Contains("rabbit", StringComparison.OrdinalIgnoreCase) => "🐰",
        ContainerResource c when c.Name.Contains("kafka", StringComparison.OrdinalIgnoreCase) => "📨",
        ContainerResource c when c.Name.Contains("mail", StringComparison.OrdinalIgnoreCase) => "📧",
        ContainerResource c when c.Name.Contains("minecraft", StringComparison.OrdinalIgnoreCase) => "⛏️",
        ContainerResource => "📦",
        ProjectResource => "🔷",
        ExecutableResource => "⚙️",
        _ => "✅"
    };

    /// <summary>
    /// Returns a helpful description based on the resource type.
    /// </summary>
    private static string GetResourceDescription(IResource resource) => resource switch
    {
        ContainerResource c => $"Container `{c.Name}` started successfully.",
        ProjectResource p => $".NET project `{p.Name}` is running.",
        ExecutableResource e => $"Executable `{e.Name}` is running.",
        _ => "Resource started successfully."
    };
}

/// <summary>
/// Discord embed color values (in decimal format).
/// These create nice visual distinction in the Discord channel.
/// </summary>
internal static class DiscordColors
{
    public const int Green = 0x57F287;   // Success
    public const int Red = 0xED4245;     // Error/Crash
    public const int Blue = 0x5865F2;    // Info/Starting
    public const int Purple = 0x9B59B6;  // Config/Watching
    public const int Orange = 0xE67E22;  // Warning
    public const int Gray = 0x95A5A6;    // Neutral/Stopped
}
