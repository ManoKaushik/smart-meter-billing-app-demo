using Microsoft.Extensions.Options;
using SmartMeterWeb.Configs;
using SmartMeterWeb.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;


namespace SmartMeterWeb.Services
{
    public class MailService:IMailService
    {
        private readonly EmailSettings _emailSettings;

        public MailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.None);
            if (!string.IsNullOrEmpty(_emailSettings.Password))
            {
                await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
            }
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
