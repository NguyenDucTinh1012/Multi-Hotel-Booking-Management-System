using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Backend.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body)
        {
            var message = new MimeMessage();

            // Người gửi
            message.From.Add(
                new MailboxAddress(
                    _settings.SenderName,
                    _settings.SenderEmail
                )
            );

            // Người nhận
            message.To.Add(
                MailboxAddress.Parse(toEmail)
            );

            // Tiêu đề
            message.Subject = subject;

            // Nội dung email HTML
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            message.Body = bodyBuilder.ToMessageBody();

            // Kết nối Gmail SMTP
            using var client = new SmtpClient();

            await client.ConnectAsync(
                _settings.SmtpServer,
                _settings.Port,
                SecureSocketOptions.StartTls
            );

            // Xác thực tài khoản Gmail
            await client.AuthenticateAsync(
                _settings.Username,
                _settings.Password
            );

            // Gửi email
            await client.SendAsync(message);

            // Ngắt kết nối
            await client.DisconnectAsync(true);
        }
    }
}
