namespace AspireAllTheThings.AppHost;

/// <summary>
/// DEMO 1: Simple Docker Container - IT-Tools
/// 
/// This demonstrates the simplest way to add ANY Docker image to Aspire.
/// IT-Tools is a collection of 50+ handy developer utilities including:
/// - Hash generators, JSON formatters, Base64 encoders
/// - UUID generators, QR code tools, and more!
/// 
/// Key Concepts:
/// - AddContainer() - Add ANY Docker image to Aspire
/// - WithHttpEndpoint() - Expose HTTP ports
/// - WithExternalHttpEndpoints() - Make accessible from the dashboard
/// </summary>
public static class ItToolsDemo
{
    public static IDistributedApplicationBuilder AddItToolsDemo(this IDistributedApplicationBuilder builder)
    {
        var itTools = builder.AddContainer("it-tools", "corentinth/it-tools")
            .WithHttpEndpoint(targetPort: 80, name: "http")
            .WithExternalHttpEndpoints();

        return builder;
    }
}
