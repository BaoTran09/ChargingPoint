using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Newtonsoft.Json;
using System.Data;

namespace ChargingApp.Helpers
{
    /// <summary>
    /// Email configuration model
    /// </summary>
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
    }

    /// <summary>
    /// Email message model
    /// </summary>
    public class EmailMessage
    {
        public string To { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = false;
        public List<string> AttachmentPaths { get; set; } = new List<string>();
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
    }

    /// <summary>
    /// Email Helper - Centralized email sending service
    /// </summary>
    public static class EmailHelper
    {
        private static EmailSettings _settings;
        private static readonly object _lock = new object();

        /// <summary>
        /// Load settings từ EmailSettings.json
        /// </summary>
        public static EmailSettings LoadSettings()
        {
            if (_settings != null)
                return _settings;

            lock (_lock)
            {
                if (_settings != null)
                    return _settings;

                try
                {
                    // Tìm file EmailSettings.json
                    string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailSettings.json");

                    if (!File.Exists(settingsPath))
                    {
                        // Tạo file mẫu nếu chưa có
                        CreateDefaultSettingsFile(settingsPath);
                    }

                    string json = File.ReadAllText(settingsPath);
                    _settings = JsonConvert.DeserializeObject<EmailSettings>(json);

                    return _settings;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi load EmailSettings.json: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Reload settings từ file (dùng khi thay đổi config)
        /// </summary>
        public static void ReloadSettings()
        {
            lock (_lock)
            {
                _settings = null;
                LoadSettings();
            }
        }

        /// <summary>
        /// Tạo file EmailSettings.json mẫu
        /// </summary>
        private static void CreateDefaultSettingsFile(string path)
        {
            var defaultSettings = new EmailSettings
            {
                SmtpServer = "smtp.gmail.com",
                SmtpPort = 587,
                EnableSsl = true,
                SenderEmail = "tranne2k4@gmail.com",
                SenderName = "CÔNG TY CỔ PHẦN PHÁT TRIỂN TRẠM SẠC TOÀN CẦU V-GREENn",
                Username = "tranne2k4@gmail.com",
                Password = "mjpw uktq mbyu qrkz",
                TimeoutSeconds = 30
           };

            string json = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Gửi email đơn giản
        /// </summary>
        // Trong EmailHelper.cs

        public static bool SendEmail(
            string toEmail,
            string subject,
            string body,
            out string errorMessage,      // <--- CHUYỂN LÊN ĐÂY (Trước tham số tùy chọn)
            string attachmentPath = null  // <--- CHUYỂN XUỐNG CUỐI (Vì có giá trị mặc định)
        )
        {
            errorMessage = null;

            try
            {
                var message = new EmailMessage
                {
                    To = toEmail,
                    Subject = subject,
                    Body = body,
                    IsHtml = false
                };

                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    message.AttachmentPaths.Add(attachmentPath);
                }

                return SendEmail(message, out errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Gửi email với EmailMessage object (advanced)
        /// </summary>
        public static bool SendEmail(EmailMessage emailMessage, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(emailMessage.To))
                {
                    errorMessage = "Email người nhận không được để trống!";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(emailMessage.Subject))
                {
                    errorMessage = "Tiêu đề email không được để trống!";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(emailMessage.Body))
                {
                    errorMessage = "Nội dung email không được để trống!";
                    return false;
                }

                // Load settings
                var settings = LoadSettings();

                if (string.IsNullOrEmpty(settings.SmtpServer))
                {
                    errorMessage = "Chưa cấu hình SMTP Server trong EmailSettings.json!";
                    return false;
                }

                // Create MimeMessage
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));

                // To
                message.To.Add(string.IsNullOrEmpty(emailMessage.ToName)
                    ? new MailboxAddress(emailMessage.To, emailMessage.To)
                    : new MailboxAddress(emailMessage.ToName, emailMessage.To));

                // Cc
                foreach (var cc in emailMessage.Cc.Where(c => !string.IsNullOrWhiteSpace(c)))
                {
                    message.Cc.Add(new MailboxAddress(cc, cc));
                }

                // Bcc
                foreach (var bcc in emailMessage.Bcc.Where(b => !string.IsNullOrWhiteSpace(b)))
                {
                    message.Bcc.Add(new MailboxAddress(bcc, bcc));
                }

                message.Subject = emailMessage.Subject;

                // Body builder
                var builder = new BodyBuilder();

                if (emailMessage.IsHtml)
                {
                    builder.HtmlBody = emailMessage.Body;
                }
                else
                {
                    builder.TextBody = emailMessage.Body;
                }

                // Attachments
                foreach (var attachmentPath in emailMessage.AttachmentPaths.Where(a => File.Exists(a)))
                {
                    try
                    {
                        builder.Attachments.Add(attachmentPath);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi đính kèm file {attachmentPath}: {ex.Message}");
                    }
                }

                message.Body = builder.ToMessageBody();

                // Send via SMTP
                using (var client = new SmtpClient())
                {
                    client.Timeout = settings.TimeoutSeconds * 1000;

                    // Connect
                    var secureOptions = settings.EnableSsl
                        ? SecureSocketOptions.StartTls
                        : SecureSocketOptions.None;

                    client.Connect(settings.SmtpServer, settings.SmtpPort, secureOptions);

                    // Authenticate
                    if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
                    {
                        client.Authenticate(settings.Username, settings.Password);
                    }

                    // Send
                    client.Send(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Lỗi gửi email: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"EmailHelper Error: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Gửi email hóa đơn
        /// </summary>
        public static bool SendInvoiceEmail(
            string toEmail,
            string toName,
            long invoiceId,
            string pdfPath,
            out string errorMessage)
        {
            string subject = $"Hóa đơn #{invoiceId} - V-GREEN Charging Station";
            string body = $@"Kính gửi {toName},

                Chúng tôi xin gửi đến Quý khách hóa đơn thanh toán dịch vụ sạc điện.

                Chi tiết hóa đơn xin xem file đính kèm.

                Mọi thắc mắc xin vui lòng liên hệ:
                - Hotline: 1900 1234
                - Email: support@vgreen.com

                Trân trọng cảm ơn!
                V-GREEN Charging Station";

            return SendEmail(toEmail, subject, body, out errorMessage, pdfPath);
        }

        /// <summary>
        /// Gửi email nhắc nợ
        /// </summary>
        public static bool SendReminderEmail(
            string toEmail,
            string toName,
            long invoiceId,
            decimal amount,
            DateTime dueDate,
            string pdfPath,
            out string errorMessage)
        {
            string subject = $"Nhắc nhở thanh toán - Hóa đơn #{invoiceId}";
            string body = $@"Kính gửi {toName},

                                Chúng tôi xin gửi đến Quý khách thông báo nhắc nhở thanh toán hóa đơn:

                                - Số hóa đơn: #{invoiceId}
                                - Số tiền: {amount:N0} VND
                                - Hạn thanh toán: {dueDate:dd/MM/yyyy}
                                - Trạng thái: Chưa thanh toán

                                Vui lòng thanh toán hóa đơn trong thời gian sớm nhất để tránh phát sinh phí phạt.

                                Chi tiết hóa đơn xin xem file đính kèm.

                                Mọi thắc mắc xin vui lòng liên hệ:
                                - Hotline: 1900 1234
                                - Email: support@vgreen.com

                                Trân trọng cảm ơn!
                                V-GREEN Charging Station";

            return SendEmail(toEmail, subject, body, out errorMessage, pdfPath);
        }

        /// <summary>
        /// Test connection
        /// </summary>
        public static bool TestConnection(out string errorMessage)
        {
            errorMessage = null;

            try
            {
                var settings = LoadSettings();

                using (var client = new SmtpClient())
                {
                    client.Timeout = settings.TimeoutSeconds * 1000;

                    var secureOptions = settings.EnableSsl
                        ? SecureSocketOptions.StartTls
                        : SecureSocketOptions.None;

                    client.Connect(settings.SmtpServer, settings.SmtpPort, secureOptions);

                    if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
                    {
                        client.Authenticate(settings.Username, settings.Password);
                    }

                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Lỗi kết nối SMTP: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Tạo nội dung email cho Receipt (Thanh toán thành công)
        /// </summary>
        public static string GetReceiptEmailBody(DataRow receiptData)
        {
            string receiptNumber = receiptData["ReceiptNumber"]?.ToString() ?? "";
            string customerName = receiptData["CustomerName"]?.ToString() ?? "Quý khách";
            decimal totalAmount = Convert.ToDecimal(receiptData["TotalAmount"]);
            DateTime receiptDate = Convert.ToDateTime(receiptData["ReceiptDate"]);
            string paymentMethod = receiptData["PaymentMethod"]?.ToString() ?? "";
            string description = receiptData["Description"]?.ToString() ?? "";

            string emailBody = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='utf-8'>
                            <style>
                                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                                .header {{ background: #28a745; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
                                .content {{ background: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
                                .info-row {{ margin: 10px 0; padding: 10px; background: white; border-left: 3px solid #28a745; }}
                                .label {{ font-weight: bold; color: #555; }}
                                .value {{ color: #333; }}
                                .amount {{ font-size: 24px; color: #28a745; font-weight: bold; text-align: center; padding: 15px; background: white; margin: 15px 0; border-radius: 5px; }}
                                .footer {{ background: #f1f1f1; padding: 15px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 5px 5px; }}
                                .success-icon {{ font-size: 48px; text-align: center; color: #28a745; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <div class='success-icon'>✓</div>
                                    <h2>XÁC NHẬN THANH TOÁN THÀNH CÔNG</h2>
                                </div>
        
                                <div class='content'>
                                    <p>Kính gửi <strong>{customerName}</strong>,</p>
            
                                    <p>Cảm ơn Quý khách đã thanh toán. Chúng tôi xin xác nhận đã nhận được khoản thanh toán của Quý khách với thông tin như sau:</p>
            
                                    <div class='info-row'>
                                        <span class='label'>Số phiếu thu:</span>
                                        <span class='value'>{receiptNumber}</span>
                                    </div>
            
                                    <div class='info-row'>
                                        <span class='label'>Ngày thu:</span>
                                        <span class='value'>{receiptDate:dd/MM/yyyy HH:mm}</span>
                                    </div>
            
                                    <div class='info-row'>
                                        <span class='label'>Phương thức thanh toán:</span>
                                        <span class='value'>{paymentMethod}</span>
                                    </div>";

                                    if (!string.IsNullOrEmpty(description))
                                    {
                                        emailBody += $@"
                                    <div class='info-row'>
                                        <span class='label'>Nội dung:</span>
                                        <span class='value'>{description}</span>
                                    </div>";
                                    }

                                    emailBody += $@"
                                    <div class='amount'>
                                        Số tiền: {totalAmount:N0} VNĐ
                                    </div>
            
                                    <p>Phiếu thu chi tiết được đính kèm theo email này.</p>
            
                                    <p>Nếu có bất kỳ thắc mắc nào, xin vui lòng liên hệ với chúng tôi.</p>
            
                                    <p>Trân trọng cảm ơn!</p>
                                </div>
        
                                <div class='footer'>
                                    <p><strong>CÔNG TY XYZ</strong></p>
                                    <p>Địa chỉ | Điện thoại | Email</p>
                                    <p><em>Email này được gửi tự động, vui lòng không trả lời.</em></p>
                                </div>
                            </div>
                        </body>
                        </html>";

            return emailBody;
        }

        /// <summary>
        /// Validate email address
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}