using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.ViewModels
{
    public class ChangePasswordView
    {

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }  // Email người dùng

        [Required]
        public string Token { get; set; } // Token xác thực để thay đổi mật khẩu

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(40, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 8)]
        [Compare("ConfirmNewPassword", ErrorMessage = "The password and confirmation password do not match.")]

        public string NewPassword { get; set; } // Mật khẩu người dùng


        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; } // Xác nhận mật khẩu

    }
}
