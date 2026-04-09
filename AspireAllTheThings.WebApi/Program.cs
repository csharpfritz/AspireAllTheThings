using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddOpenApi();

// Register the AI chat client from GitHub Models when Part 7 is enabled
// (connection is only available when AddGitHubModelDemo() is uncommented in AppHost)
if (builder.Configuration.GetConnectionString("chat") is not null)
{
    builder.AddAzureChatCompletionsClient("chat")
        .AddChatClient();
}

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// AI Chat endpoint - uses GitHub Models via IChatClient
// Only available when Part 7 (AddGitHubModelDemo) is enabled in AppHost
app.MapGet("/chat", async (IChatClient? chatClient, string? message) =>
{
    if (chatClient is null)
        return Results.Problem("AI not configured. Enable Part 7 in AppHost.cs and set the GitHub API key.");

    var prompt = message ?? "Hello! What can you help me with today?";
    var response = await chatClient.GetResponseAsync(prompt);
    return Results.Ok(new { prompt, response = response.Text });
})
.WithName("Chat");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
