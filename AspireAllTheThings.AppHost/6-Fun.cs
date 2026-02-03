namespace AspireAllTheThings.AppHost;

/// <summary>
/// PART 6: Fun Examples - Minecraft!
/// 
/// Because Aspire isn't just for web apps and databases - it can
/// orchestrate ANY containerized workload. What better way to prove
/// that than running Minecraft servers!
/// 
/// This demonstrates:
/// - Demo 6a: Standard Minecraft server with custom MOTD
/// 
/// Both are excluded from the manifest so they won't be published to
/// production - these are dev/demo tools only.
/// </summary>
public static class FunDemo
{
    /// <summary>
    /// Demo 6a: Minecraft Server
    /// 
    /// Uses itzg/minecraft-server - the most popular Minecraft Docker image
    /// with 100M+ pulls. Shows environment variable configuration.
    /// 
    /// Key Concepts:
    /// - WithEnvironment() - Pass configuration to containers
    /// - WithEndpoint() - Expose non-HTTP ports (TCP)
    /// - WithVolume() - Persist data between restarts
    /// - ExcludeFromManifest() - Dev-only, don't publish
    /// 
    /// Connect: localhost:25565
    /// </summary>
    public static IDistributedApplicationBuilder AddMinecraftDemo(this IDistributedApplicationBuilder builder)
    {
        var minecraft = builder.AddContainer("minecraft", "itzg/minecraft-server")
            .WithEnvironment("EULA", "TRUE")
            .WithEnvironment("MODE", "creative")
            .WithEnvironment("DIFFICULTY", "peaceful")
            .WithEnvironment("MOTD", "Swetugg Stockholm 2026 - Aspire All The Things!")
            .WithEndpoint(targetPort: 25565, port: 25565, name: "minecraft", scheme: "tcp")
            .WithVolume("minecraft-data", "/data")
            .ExcludeFromManifest();  // Don't publish - dev/demo only!

        return builder;
    }

}
