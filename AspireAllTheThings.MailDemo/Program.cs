using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();

// Configure SmtpClient to use MailPit
// MailPit connection provides: smtp://{host}:{port}
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var mailpitConnection = configuration.GetConnectionString("mailpit");
    
    if (string.IsNullOrEmpty(mailpitConnection))
    {
        // Default fallback for local development
        return new SmtpClient("localhost", 1025)
        {
            EnableSsl = false,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
    }
    
    // Parse the connection string (format: Endpoint=smtp://host:port)
    var endpointValue = mailpitConnection;
    if (mailpitConnection.StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase))
    {
        endpointValue = mailpitConnection.Substring("Endpoint=".Length);
    }
    
    var uri = endpointValue.StartsWith("smtp://", StringComparison.OrdinalIgnoreCase) 
        ? new Uri(endpointValue) 
        : new Uri($"smtp://{endpointValue}");
    
    return new SmtpClient(uri.Host, uri.Port > 0 ? uri.Port : 1025)
    {
        EnableSsl = false,
        DeliveryMethod = SmtpDeliveryMethod.Network
    };
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
