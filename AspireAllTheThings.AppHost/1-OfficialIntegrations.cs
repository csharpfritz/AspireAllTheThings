namespace AspireAllTheThings.AppHost;

/// <summary>
/// PART 1: Official Aspire Integrations
/// 
/// These are the integrations published and maintained by the .NET Aspire team.
/// They provide first-class support for popular databases, caches, and Azure services.
/// 
/// Each integration provides:
/// - Automatic container provisioning (for local dev)
/// - Connection string management
/// - Health checks
/// - OpenTelemetry instrumentation
/// - Azure resource provisioning (for deployment)
/// </summary>
public static class OfficialIntegrationsDemo
{
    /// <summary>
    /// Demo: Redis Cache
    /// 
    /// Redis is an in-memory data store used for caching, session storage,
    /// and pub/sub messaging. Aspire makes it trivial to add.
    /// 
    /// Package: Aspire.Hosting.Redis
    /// </summary>
    public static IDistributedApplicationBuilder AddRedisDemo(this IDistributedApplicationBuilder builder)
    {
        var redis = builder.AddRedis("cache")
            .WithRedisInsight();  // Adds Redis Insight UI for debugging

        return builder;
    }

    /// <summary>
    /// Demo: PostgreSQL Database
    /// 
    /// PostgreSQL is a powerful open-source relational database.
    /// Aspire can provision it locally and deploy to Azure Database for PostgreSQL.
    /// 
    /// Package: Aspire.Hosting.PostgreSQL
    /// </summary>
    public static IDistributedApplicationBuilder AddPostgresDemo(this IDistributedApplicationBuilder builder)
    {
        var postgres = builder.AddPostgres("postgres")
            .WithPgAdmin()  // Adds pgAdmin UI for database management
            .AddDatabase("catalogdb");

        return builder;
    }

    /// <summary>
    /// Demo: SQL Server Database
    /// 
    /// SQL Server is Microsoft's enterprise relational database.
    /// Perfect for .NET applications with Entity Framework Core.
    /// 
    /// Package: Aspire.Hosting.SqlServer
    /// </summary>
    public static IDistributedApplicationBuilder AddSqlServerDemo(this IDistributedApplicationBuilder builder)
    {
        var sqlserver = builder.AddSqlServer("sql")
            .AddDatabase("ordersdb");

        return builder;
    }

    /// <summary>
    /// Demo: Azure Service Bus
    /// 
    /// Azure Service Bus is a fully managed enterprise message broker.
    /// Great for decoupling services with queues and topics.
    /// 
    /// Note: Uses Azure emulator for local development!
    /// Package: Aspire.Hosting.Azure.ServiceBus
    /// </summary>
    public static IDistributedApplicationBuilder AddAzureServiceBusDemo(this IDistributedApplicationBuilder builder)
    {
        var serviceBus = builder.AddAzureServiceBus("messaging")
            .RunAsEmulator();  // Use emulator for local dev
        
        serviceBus.AddServiceBusQueue("orders");
        serviceBus.AddServiceBusTopic("events")
            .AddServiceBusSubscription("audit");  // Topic with subscription

        return builder;
    }

    /// <summary>
    /// Demo: Azure Cosmos DB
    /// 
    /// Azure Cosmos DB is a globally distributed NoSQL database.
    /// Supports multiple APIs: SQL, MongoDB, Cassandra, Gremlin, Table.
    /// 
    /// Note: Uses Azure emulator for local development!
    /// Package: Aspire.Hosting.Azure.CosmosDB
    /// </summary>
    public static IDistributedApplicationBuilder AddAzureCosmosDbDemo(this IDistributedApplicationBuilder builder)
    {
        var cosmos = builder.AddAzureCosmosDB("cosmos")
            .RunAsEmulator()  // Use emulator for local dev
            .AddCosmosDatabase("appdata");

        return builder;
    }

    /// <summary>
    /// Demo: Azure Storage (Blobs, Queues, Tables)
    /// 
    /// Azure Storage provides scalable cloud storage for blobs, queues, and tables.
    /// Uses Azurite emulator for local development.
    /// 
    /// Package: Aspire.Hosting.Azure.Storage
    /// </summary>
    public static IDistributedApplicationBuilder AddAzureStorageDemo(this IDistributedApplicationBuilder builder)
    {
        var storage = builder.AddAzureStorage("storage")
            .RunAsEmulator();  // Use Azurite for local dev
        
        var blobs = storage.AddBlobs("blobs");
        var queues = storage.AddQueues("queues");
        var tables = storage.AddTables("tables");

        return builder;
    }

    /// <summary>
    /// Add ALL official integration demos at once
    /// </summary>
    public static IDistributedApplicationBuilder AddAllOfficialIntegrations(this IDistributedApplicationBuilder builder)
    {
        return builder
            .AddRedisDemo()
            .AddPostgresDemo()
            .AddSqlServerDemo()
            .AddAzureServiceBusDemo()
            .AddAzureCosmosDbDemo()
            .AddAzureStorageDemo();
    }
}
