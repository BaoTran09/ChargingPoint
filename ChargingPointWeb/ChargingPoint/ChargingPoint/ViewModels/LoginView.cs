using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.ViewModels
{
    public class LoginView
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }  // Email người dùng


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }// Mật khẩu người dùng

        [Display(Name = "Tên đăng nhập (nhân viên)")]
        public string? Username { get; set; } // Chỉ dùng khi là nhân viên

        [Required]
        public string UserType { get; set; } = "Customer"; // "Customer" | "Employee"
        [Required(ErrorMessage = "Remember me option is required")]
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }// Lưu đăng nhập hay khôngEmail
        //public string ReturnUrl { get; set; } = "/"; // Đường dẫn trả về sau khi đăng nhập thành công
     
    }
}
