namespace AspireAllTheThings.AppHost;

/// <summary>
/// DEMO 5: Fun Examples - Minecraft!
/// 
/// Because Aspire isn't just for web apps and databases - it can
/// orchestrate ANY containerized workload. What better way to prove
/// that than running Minecraft servers!
/// 
/// This file contains two Minecraft demos:
/// - Demo 5a: Standard Minecraft server with custom MOTD
/// - Demo 5b: Dockercraft - visualize ALL Aspire containers in Minecraft!
/// 
/// Both are excluded from the manifest so they won't be published to
/// production - these are dev/demo tools only.
/// </summary>
public static class FunDemo
{
    /// <summary>
    /// Demo 5a: Minecraft Server with Custom MOTD
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

    /// <summary>
    /// Demo 5b: Dockercraft - Visualize Aspire Containers in Minecraft!
    /// 
    /// Dockercraft connects to the Docker socket and renders EVERY running
    /// container as a structure in the Minecraft world. Walk around and see
    /// IT-Tools, MailPit, Minecraft server, and Dockercraft itself as buildings!
    /// 
    /// Interactive Features:
    /// - Pull levers to start/stop containers
    /// - Push buttons to remove containers
    /// - Use chat (T key) for Docker commands: /docker ps
    /// 
    /// Key Concepts:
    /// - WithBindMount() - Mount host paths into containers
    /// - Docker socket access for container visualization
    /// - Proves Aspire is "just Docker" under the hood
    /// 
    /// Connect: localhost:25566
    /// Learn More: https://github.com/docker/dockercraft
    /// 
    /// NOTE: Requires Docker socket access
    /// - Linux/macOS: /var/run/docker.sock
    /// - Windows Docker Desktop: //var/run/docker.sock
    /// </summary>
    public static IDistributedApplicationBuilder AddDockercraftDemo(this IDistributedApplicationBuilder builder)
    {
        var dockercraft = builder.AddContainer("dockercraft", "gaetan/dockercraft")
            .WithEndpoint(targetPort: 25565, port: 25566, name: "dockercraft", scheme: "tcp")
            .WithBindMount("/var/run/docker.sock", "/var/run/docker.sock")
            .WithArgs("Forest", "63", "0", "Trees")  // Biome: Forest with trees!
            .ExcludeFromManifest();  // Don't publish - dev/demo only!
        // For Windows Docker Desktop, you might need:
        // .WithBindMount("//var/run/docker.sock", "/var/run/docker.sock");

        return builder;
    }
}
