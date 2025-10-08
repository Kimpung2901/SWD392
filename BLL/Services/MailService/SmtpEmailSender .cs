using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using BLL.IService; // ✅ Import đúng namespace

namespace BLL.Services.MailService
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _cfg;
        public SmtpEmailSender(IConfiguration cfg) => _cfg = cfg;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage, string? displayName = null)
        {
            var host = _cfg["Smtp:Host"]!;
            var port = _cfg.GetValue<int>("Smtp:Port");
            var user = _cfg["Smtp:User"]!;
            var pass = _cfg["Smtp:Pass"]!;
            var display = displayName ?? _cfg["Smtp:Display"] ?? "Doll Store";

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var from = new MailAddress(user, display); 
            var to = new MailAddress(email);

            using var msg = new MailMessage(from, to)
            {
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            await client.SendMailAsync(msg);
        }
    }
}
