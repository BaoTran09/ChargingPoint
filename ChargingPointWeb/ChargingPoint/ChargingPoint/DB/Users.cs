using Microsoft.AspNetCore.Identity;

namespace ChargingPoint.DB
{
    public class Users : IdentityUser
    {
       
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;    // DATETIME2(3), nullable

        public string? Status { get; set; }           // VARCHAR(50), ví dụ: Active, Inactive, Locked

    }
}
