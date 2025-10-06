using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

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

public class SmtpEmailSender
{
    private readonly MailSettings _cfg;
    public SmtpEmailSender(IOptions<MailSettings> cfg) => _cfg = cfg.Value;

    public async Task SendAsync(string to, string subject, string htmlBody, string? displayName = null)
    {
        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("Recipient (to) is required.", nameof(to));

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
            Body = htmlBody ?? string.Empty,
            IsBodyHtml = true
        };
        msg.To.Add(to);

        await client.SendMailAsync(msg);
    }
}
