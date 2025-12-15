using ChargingPoint.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.DB
{
    public class Users : IdentityUser
    {
        // Username riêng cho Employee (khác với UserName của Identity)
        [StringLength(50)]
        public string? EmployeeUsername { get; set; }

        public string? Status { get; set; } = "Active";

        // Navigation properties
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;    // DATETIME2(3), nullable


        public virtual Employee Employee { get; set; }
        public virtual Customer Customer { get; set; }
    }
    }

