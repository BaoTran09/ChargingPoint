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
            var userame = _configuration["EmailSettings:UserName"];
            var Password = _configuration["EmailSettings:Password"];
            var Message = new MailMessage(from!, toEmail, subject, body);
            Message.IsBodyHtml = true;
            using var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new System.Net.NetworkCredential(userame!, Password!),
                EnableSsl = true
            };


            await client.SendMailAsync(Message);

        }



    }

}
