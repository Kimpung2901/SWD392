using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using BLL.IService;

namespace BLL.Services.MailService;

public class MailSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? From { get; set; }
    public bool EnableSsl { get; set; } = true;
}

public class SmtpEmailSender : IEmailSender
{
    private readonly MailSettings _cfg;
    public SmtpEmailSender(IOptions<MailSettings> cfg) => _cfg = cfg.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage, string? displayName = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Recipient (email) is required.", nameof(email));

        var fromAddress = string.IsNullOrWhiteSpace(_cfg.From) ? _cfg.UserName : _cfg.From;
        if (string.IsNullOrWhiteSpace(fromAddress))
            throw new InvalidOperationException("Mail.From or Mail.UserName must be configured.");

        using var client = new SmtpClient(_cfg.Host, _cfg.Port)
        {
            Credentials = new NetworkCredential(_cfg.UserName, _cfg.Password),
            EnableSsl = _cfg.EnableSsl
        };

        var from = string.IsNullOrWhiteSpace(displayName)
            ? new MailAddress(fromAddress)
            : new MailAddress(fromAddress, displayName);

        using var msg = new MailMessage()
        {
            From = from,
            Subject = subject ?? string.Empty,
            Body = htmlMessage ?? string.Empty,
            IsBodyHtml = true
        };
        msg.To.Add(email);

        await client.SendMailAsync(msg);
    }
}
