using Aspire.Hosting.ApplicationModel;

namespace AspireAllTheThings.AppHost;

/// <summary>
/// PART 2: Multi-Language Application Support
/// 
/// Aspire isn't just for .NET! It can orchestrate applications written in
/// any language. This demo shows ASP.NET, Python, and Node.js apps all
/// managed together by Aspire.
/// 
/// Key Concepts:
/// - AddProject() - Add .NET projects
/// - AddPythonApp() - Add Python applications (requires Python SDK)
/// - AddNpmApp() - Add Node.js applications
/// - WithReference() - Connect services together
/// - WaitFor() - Define startup dependencies
/// </summary>
public static class MultiLanguageDemo
{
    /// <summary>
    /// Demo: ASP.NET Core Web API
    /// 
    /// A simple ASP.NET Core API that can connect to Redis and PostgreSQL.
    /// Shows how .NET projects integrate seamlessly with Aspire resources.
    /// </summary>
    public static IDistributedApplicationBuilder AddAspNetApiDemo(this IDistributedApplicationBuilder builder)
    {
        var api = builder.AddProject<Projects.AspireAllTheThings_WebApi>("webapi")
            .WithExternalHttpEndpoints();

        return builder;
    }

    /// <summary>
    /// Demo: Python Flask API
    /// 
    /// A Python Flask application managed by Aspire.
    /// Shows that Aspire works with non-.NET languages!
    /// 
    /// Requirements:
    /// - Python installed on the system
    /// - Run: pip install -r requirements.txt (or use virtual environment)
    /// </summary>
    public static IDistributedApplicationBuilder AddPythonApiDemo(this IDistributedApplicationBuilder builder)
    {
        var pythonApp = builder.AddPythonApp("python-api", "../python-api", "app.py")
            .WithHttpEndpoint(targetPort: 5000, name: "http")
            .WithExternalHttpEndpoints();

        return builder;
    }

    /// <summary>
    /// Demo: Node.js Express API
    /// 
    /// A Node.js Express application managed by Aspire.
    /// Shows npm/Node.js integration with Aspire orchestration.
    /// 
    /// Requirements:
    /// - Node.js installed on the system
    /// - Run: npm install in the node-api folder
    /// </summary>
    public static IDistributedApplicationBuilder AddNodeApiDemo(this IDistributedApplicationBuilder builder)
    {
        var nodeApp = builder.AddNpmApp("node-api", "../node-api", "start")
            .WithHttpEndpoint(port: 3000, env: "PORT")
            .WithExternalHttpEndpoints();

        return builder;
    }

    /// <summary>
    /// Add ALL multi-language demos at once
    /// Shows .NET, Python, and Node.js apps all managed by Aspire
    /// </summary>
    public static IDistributedApplicationBuilder AddAllMultiLanguageDemos(this IDistributedApplicationBuilder builder)
    {
        return builder
            .AddAspNetApiDemo()
            .AddPythonApiDemo()
            .AddNodeApiDemo();
    }
}
