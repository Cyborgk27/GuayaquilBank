using GuayaquilBank.Domain.Interfaces;
using GuayaquilBank.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace GuayaquilBank.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<InfrastructureSettings> options)
        {
            _settings = options.Value.Email;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(_settings.FromEmail, to, subject, body);
            await client.SendMailAsync(mailMessage);
        }
    }
}
