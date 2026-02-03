namespace AspireAllTheThings.AppHost;

/// <summary>
/// DEMO 2: Community Toolkit Integration - MailPit
/// 
/// This demonstrates the Aspire Community Toolkit - a collection of
/// pre-built integrations maintained by the community.
/// 
/// MailPit is a lightweight email testing tool. Instead of manually
/// configuring a container with ports and environment variables,
/// the Community Toolkit gives us a single method call!
/// 
/// Key Concepts:
/// - Community Toolkit simplifies common integrations
/// - One line of code instead of manual container configuration
/// - First-class integration with health checks, service discovery, etc.
/// - Demo app shows actual email sending captured by MailPit
/// 
/// Package: CommunityToolkit.Aspire.Hosting.Mailpit
/// Learn More: https://github.com/CommunityToolkit/Aspire
/// </summary>
public static class MailPitDemo
{
    public static IDistributedApplicationBuilder AddMailPitDemo(this IDistributedApplicationBuilder builder)
    {
        // Add MailPit email testing server
        var mailpit = builder.AddMailPit("mailpit");

        // Add a demo web app that can send emails through MailPit
        builder.AddProject<Projects.AspireAllTheThings_MailDemo>("maildemo")
            .WithReference(mailpit);

        return builder;
    }
}
