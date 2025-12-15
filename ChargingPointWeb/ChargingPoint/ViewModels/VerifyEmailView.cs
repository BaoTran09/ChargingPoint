
using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.ViewModels
{
    public class VerifyEmailView
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }  // Email người dùng


    }
}
