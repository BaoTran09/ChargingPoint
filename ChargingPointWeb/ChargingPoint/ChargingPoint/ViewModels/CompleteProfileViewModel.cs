using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.ViewModels
{
    public class CompleteProfileViewModel
    {
        
            public string UserId { get; set; }
            public string Token { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập họ tên")]
            public string FullName { get; set; }

            [Phone]
            public string? PhoneNumber { get; set; }

            public DateTime? Birthday { get; set; }

            public string? Address { get; set; }
        
    }
}
