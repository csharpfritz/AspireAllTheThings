namespace AspireAllTheThings.AppHost;

/// <summary>
/// PART 6: AI Integration with GitHub Models
/// 
/// GitHub Models provides access to AI models (like GPT-4o-mini) through
/// GitHub's inference API. This demo shows how to add AI capabilities
/// to your Aspire application with just a few lines of code.
/// 
/// Key Features:
/// - GenAI Visualizer: The Aspire dashboard automatically shows AI telemetry
///   (token usage, latency, prompts/completions) when OpenTelemetry is flowing
/// - Interactive Parameter Prompts: The API key uses secret: true, so the
///   dashboard will prompt for it interactively at startup
/// - IChatClient Abstraction: Uses Microsoft.Extensions.AI for a clean,
///   provider-agnostic AI client interface
/// 
/// Packages needed:
/// - AppHost: Aspire.Hosting.GitHub.Models
/// - Client:  Aspire.Azure.AI.Inference (registers IChatClient via DI)
/// </summary>
public static class AiDemo
{
    /// <summary>
    /// Demo: GitHub Models - AI Chat
    /// 
    /// Adds a GitHub Models resource (GPT-4o-mini) and wires it to the WebApi
    /// project. The API key is configured as a secret parameter, which triggers
    /// the Aspire dashboard's interactive parameter prompt feature.
    /// 
    /// The GenAI visualizer in the Aspire dashboard will automatically appear
    /// when AI inference telemetry flows through OpenTelemetry.
    /// 
    /// Setup:
    /// - Create a GitHub Personal Access Token with "models: read" permission
    /// - Either set it in user secrets or enter it in the dashboard prompt:
    ///   dotnet user-secrets set "Parameters:githubApiKey" "github_pat_YOUR_TOKEN"
    /// </summary>
    public static IDistributedApplicationBuilder AddGitHubModelDemo(this IDistributedApplicationBuilder builder)
    {


        // Register a GitHub Model resource using GPT-4o-mini
        var chat = builder.AddGitHubModel("chat", "openai/gpt-4o-mini");

        // Wire the model to the WebApi project so it can use IChatClient
        builder.AddProject<Projects.AspireAllTheThings_WebApi>("webapi")
            .WithExternalHttpEndpoints()
            .WithReference(chat)
            .WithUrlForEndpoint("http", url => { url.Url = "/chat.html"; url.DisplayText = "Chat"; });

        return builder;
    }
}
