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
/// - ExecutionContext.IsRunMode checks if running locally vs. publishing
/// - Configuration parameters allow different SMTP settings per environment
/// 
/// Package: CommunityToolkit.Aspire.Hosting.Mailpit
/// Learn More: https://github.com/CommunityToolkit/Aspire
/// </summary>
public static class MailPitDemo
{
    public static IDistributedApplicationBuilder AddMailPitDemo(this IDistributedApplicationBuilder builder)
    {
        // Add a demo web app that can send emails
        var mailDemo = builder.AddProject<Projects.AspireAllTheThings_MailDemo>("maildemo");

        if (builder.ExecutionContext.IsRunMode)
        {
            // Running locally: Use MailPit for email testing
            var mailpit = builder.AddMailPit("mailpit");
            
            // Pass MailPit's SMTP endpoint as SmtpConnection
            mailDemo.WithEnvironment("SmtpConnection", mailpit.GetEndpoint("smtp"));
        }
        else
        {
            // Publishing: Use configured SMTP connection string
            var smtpConnection = builder.AddParameter("SmtpConnection", secret: true);
            mailDemo.WithEnvironment("SmtpConnection", smtpConnection);
        }

        return builder;
    }
}
