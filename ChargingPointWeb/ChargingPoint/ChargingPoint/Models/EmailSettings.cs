using ChargingPoint.DB;

namespace ChargingPoint.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty; // Địa chỉ máy chủ SMT P
        public int Port { get; set; } = 587; // Cổng kết nối, thường là 587 cho TLS
        public string SenderName { get; set; } = string.Empty; // Tên đăng nhập SMTP  
        public string SenderEmail { get; set; } = string.Empty; // Tên đăng nhập SMTP  

        public string SenderPassword { get; set; } = string.Empty; // Mật khẩu SMTP
    }
}
