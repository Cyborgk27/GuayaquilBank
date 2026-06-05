using GuayaquilBank.Domain.Interfaces;
using GuayaquilBank.Infrastructure.Common.Settings;
using System.Net;
using System.Net.Mail;

namespace GuayaquilBank.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(InfrastructureSettings settings)
        {
            _settings = settings.Email;
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
