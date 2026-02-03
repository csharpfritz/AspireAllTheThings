using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspireAllTheThings.MailDemo.Pages;

public class IndexModel : PageModel
{
    private readonly SmtpClient _smtpClient;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(SmtpClient smtpClient, ILogger<IndexModel> logger)
    {
        _smtpClient = smtpClient;
        _logger = logger;
    }

    [BindProperty]
    public string FromEmail { get; set; } = "demo@swetugg.se";

    [BindProperty]
    public string ToEmail { get; set; } = "attendee@example.com";

    [BindProperty]
    public string Subject { get; set; } = "Hello from Swetugg Stockholm 2026! 🎉";

    [BindProperty]
    public string Body { get; set; } = @"Hi there!

This is a test email sent from the Aspire MailPit demo.

MailPit captures all outgoing emails during development, so you never accidentally send test emails to real users!

Best regards,
The Aspire Demo Team";

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // Default values are set above
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(FromEmail),
                Subject = Subject,
                Body = Body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(new MailAddress(ToEmail));

            await _smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("Email sent successfully from {From} to {To}", FromEmail, ToEmail);
            SuccessMessage = $"✅ Email sent successfully! Check the MailPit dashboard to see it.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            ErrorMessage = $"❌ Failed to send email: {ex.Message}";
        }

        return Page();
    }
}
