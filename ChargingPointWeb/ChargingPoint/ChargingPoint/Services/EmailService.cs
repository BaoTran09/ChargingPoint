using System.Net.Mail;

namespace ChargingPoint.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var from = _configuration["EmailSettings:From"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var username = _configuration["EmailSettings:UserName"];
            var password = _configuration["EmailSettings:Password"];

            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Email configuration is missing or invalid in appsettings.json.");
            }

            var message = new MailMessage(from!, toEmail, subject, body);
            message.IsBodyHtml = true;

            using var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new System.Net.NetworkCredential(username!, password!),
                EnableSsl = true
            };

            try
            {
                await client.SendMailAsync(message);
                Console.WriteLine($"Email sent successfully to {toEmail} at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {toEmail}: {ex.Message}");
                throw; // Ném lỗi để controller xử lý
            }
        }
    }
}