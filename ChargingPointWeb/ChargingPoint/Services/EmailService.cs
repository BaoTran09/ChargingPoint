using ChargingPoint.Services;
using MailKit.Security;
using MimeKit;
using System.Diagnostics.Metrics;
using System.Net.Mail;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net;
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

        public async Task SendInvoiceEmailAsync(string toEmail, string customerName, string invoiceId, byte[] pdfAttachment)
        {
            var fromEmail = _configuration["Email:From"] ?? "noreply@v-green.com";
            var fromName = "V-Green Charging";
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];

            using var message = new MailMessage();

            // From
            message.From = new MailAddress(fromEmail, fromName);

            // To
            message.To.Add(new MailAddress(toEmail, customerName));

            // Subject
            message.Subject = $"Hóa đơn sạc điện #{invoiceId} - V-Green";

            // Body
            message.Body = GetEmailTemplate(customerName, invoiceId);
            message.IsBodyHtml = true;

            // Attachment
            if (pdfAttachment != null && pdfAttachment.Length > 0)
            {
                var stream = new MemoryStream(pdfAttachment);
                var attachment = new Attachment(stream, $"Invoice_{invoiceId}.pdf", "application/pdf");
                message.Attachments.Add(attachment);
            }

            // SMTP Client
            using var smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(username, password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            // Send
            await smtpClient.SendMailAsync(message);
        }

        private string GetEmailTemplate(string customerName, string invoiceId)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }}
        .header {{
            background: linear-gradient(135deg, #0066cc 0%, #0099ff 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
        }}
        .header p {{
            margin: 10px 0 0 0;
            font-size: 16px;
            opacity: 0.9;
        }}
        .content {{
            padding: 30px;
        }}
        .content h2 {{
            color: #0066cc;
            margin-top: 0;
        }}
        .info-box {{
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .info-box strong {{
            color: #856404;
        }}
        .payment-box {{
            background-color: #f8f9fa;
            border: 2px solid #0066cc;
            padding: 20px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .payment-box h3 {{
            margin-top: 0;
            color: #0066cc;
        }}
        .payment-info {{
            font-size: 15px;
            line-height: 1.8;
        }}
        .highlight {{
            color: #dc3545;
            font-weight: bold;
            font-size: 16px;
        }}
        .warning {{
            background-color: #fff3cd;
            border: 1px solid #ffc107;
            padding: 12px;
            margin: 15px 0;
            border-radius: 4px;
            text-align: center;
        }}
        .button {{
            display: inline-block;
            padding: 12px 30px;
            background-color: #28a745;
            color: white !important;
            text-decoration: none;
            border-radius: 5px;
            margin: 20px 0;
            font-weight: bold;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #dee2e6;
        }}
        .footer p {{
            margin: 5px 0;
            color: #666;
            font-size: 13px;
        }}
        .contact-info {{
            background-color: #e7f3ff;
            padding: 15px;
            margin: 15px 0;
            border-radius: 4px;
            border-left: 4px solid #0066cc;
        }}
        table {{
            width: 100%;
            margin: 10px 0;
        }}
        table td {{
            padding: 8px 0;
        }}
        table td:first-child {{
            font-weight: bold;
            width: 40%;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔋 V-GREEN CHARGING</h1>
            <p>Trạm Sạc Điện Toàn Cầu</p>
        </div>
        
        <div class='content'>
            <h2>Xin chào {customerName},</h2>
            
            <p>Cảm ơn bạn đã sử dụng dịch vụ sạc điện của V-Green! Chúng tôi gửi kèm hóa đơn cho phiên sạc vừa hoàn thành.</p>
            
            <div class='info-box'>
                <strong>📋 Mã hóa đơn:</strong> {invoiceId}<br>
                <strong>📅 Ngày tạo:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}<br>
                <strong>📎 File đính kèm:</strong> Invoice_{invoiceId}.pdf
            </div>
            
            <div class='payment-box'>
                <h3>💳 Thông tin thanh toán</h3>
                <div class='payment-info'>
                    <table>
                        <tr>
                            <td>Chủ tài khoản:</td>
                            <td><strong>LE BAO TRAN</strong></td>
                        </tr>
                        <tr>
                            <td>Số tài khoản:</td>
                            <td><strong>1030946681</strong></td>
                        </tr>
                        <tr>
                            <td>Ngân hàng:</td>
                            <td><strong>VIETCOMBANK</strong></td>
                        </tr>
                        <tr>
                            <td>Nội dung CK:</td>
                            <td><span class='highlight'>{invoiceId}</span></td>
                        </tr>
                    </table>
                </div>
            </div>
            
            <div class='warning'>
                <strong>⚠️ LƯU Ý QUAN TRỌNG:</strong><br>
                Vui lòng thanh toán trước ngày <strong>15 tháng sau</strong> để tránh phí trễ hạn.
            </div>
            
            <div class='contact-info'>
                <strong>📞 Hỗ trợ khách hàng:</strong><br>
                Hotline: <strong>1900-xxxx</strong><br>
                Email: <strong>support@v-green.com</strong><br>
                Website: <strong>www.v-green.com</strong>
            </div>
            
            <p style='margin-top: 30px;'>
                Nếu bạn có bất kỳ thắc mắc nào, đừng ngần ngại liên hệ với chúng tôi.
            </p>
            
            <p>
                Trân trọng,<br>
                <strong>Đội ngũ V-Green Charging</strong>
            </p>
        </div>
        
        <div class='footer'>
            <p><strong>CÔNG TY CỔ PHẦN PHÁT TRIỂN TRẠM SẠC TOÀN CẦU V-GREEN</strong></p>
            <p>MST: 0123456789 | Hotline: 1900-xxxx</p>
            <p style='margin-top: 10px; color: #999;'>
                © 2025 V-Green Charging Station. All rights reserved.
            </p>
            <p style='color: #999; font-size: 11px;'>
                Email này được gửi tự động, vui lòng không trả lời trực tiếp.
            </p>
        </div>
    </div>
</body>
</html>
";
        }





























    }
}
