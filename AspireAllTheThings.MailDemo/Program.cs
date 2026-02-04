using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();

// Configure SmtpClient using SmtpConnection
// The AppHost provides this - either from MailPit (run mode) or a configured parameter (publish mode)
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var smtpConnection = configuration["SmtpConnection"];
    
    if (string.IsNullOrEmpty(smtpConnection))
    {
        // Default fallback for local development without AppHost
        return new SmtpClient("localhost", 1025)
        {
            EnableSsl = false,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
    }
    
    return ParseSmtpConnection(smtpConnection);
});

var app = builder.Build();

// Map Aspire health endpoints
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

// Helper method to parse SMTP connection strings
static SmtpClient ParseSmtpConnection(string connectionString)
{
    // Support formats:
    // - smtp://host:port
    // - smtp://user:pass@host:port
    // - host:port
    
    Uri uri;
    if (connectionString.StartsWith("smtp://", StringComparison.OrdinalIgnoreCase))
    {
        uri = new Uri(connectionString);
    }
    else
    {
        uri = new Uri($"smtp://{connectionString}");
    }
    
    var client = new SmtpClient(uri.Host, uri.Port > 0 ? uri.Port : 587)
    {
        EnableSsl = !connectionString.Contains("localhost"),
        DeliveryMethod = SmtpDeliveryMethod.Network
    };
    
    // If credentials are provided in the URI
    if (!string.IsNullOrEmpty(uri.UserInfo))
    {
        var parts = uri.UserInfo.Split(':');
        var username = Uri.UnescapeDataString(parts[0]);
        var password = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;
        client.Credentials = new NetworkCredential(username, password);
    }
    
    return client;
}
