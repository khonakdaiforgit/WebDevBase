using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyApp.Application.Abstractions.Email;
using MyApp.Infrastructure.Services.Email.Settings;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MyApp.Infrastructure.Services.Email;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<SmtpSettings> settings,
        IWebHostEnvironment env,
        ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _env = env;
        _logger = logger;
    }

    public async Task SendConfirmationEmailAsync(string to, string restaurantName, string confirmLink, CancellationToken ct = default)
    {
        var subject = $"Confirm Your Subscription to {restaurantName} Newsletter";
        var html = await LoadTemplateAsync("ConfirmationEmail.html");
        html = html
            .Replace("{{RestaurantName}}", WebUtility.HtmlEncode(restaurantName))
            .Replace("{{ConfirmLink}}", confirmLink)
            .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

        await SendEmailAsync(to, subject, html, ct);
    }

    public async Task SendNewsletterAsync(string to, string subject, string htmlContent, CancellationToken ct = default)
    {
        await SendEmailAsync(to, subject, htmlContent, ct);
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct)
    {
        try
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            var mail = new MailMessage(_settings.From, to, subject, htmlBody)
            {
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8
            };

            await client.SendMailAsync(mail, ct);
            _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    private async Task<string> LoadTemplateAsync(string templateName)
    {
        var path = Path.Combine(_env.ContentRootPath, "Infrastructure", "Services", "Email", "Templates", templateName);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Email template not found: {templateName}");

        return await File.ReadAllTextAsync(path, Encoding.UTF8);
    }
}