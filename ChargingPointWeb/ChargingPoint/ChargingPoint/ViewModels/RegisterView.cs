using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.ViewModels
{
    public class RegisterView
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }  // Email người dùng

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(40, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Password")]

        [Compare("ConfirmPassword", ErrorMessage = "The password and confirmation password do not match.")]

        public string Password { get; set; } // Mật khẩu người dùng


        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]

        public string ConfirmPassword { get; set; } // Xác nhận mật khẩu

       
    }
}
