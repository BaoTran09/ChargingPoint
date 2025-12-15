using ChargingPoint.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ChargingPoint.DB;

namespace ChargingPoint.Models
{
    [Table("Employee")]
    public class Employee
    {
        [Key]
        public long EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(200)]
        public string? JobTitle { get; set; }

        [Column(TypeName = "datetime2(3)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2(3)")]
        public DateTime? UpdatedAt { get; set; }

        [StringLength(200)]
        public string? Status { get; set; }

        public DateTime? StartingDate { get; set; }

        public int? RoleId { get; set; }
        public int? ExperienceMonth { get; set; }

        // === CŨ: liên kết Identity ===
        [StringLength(450)]
        public string? UserId { get; set; }                    // có thể null tạm thời

        // === MỚI: liên kết hệ thống nội bộ ===
        public long? UserAppId { get; set; }                   // để nullable trước

        // === MỚI: phòng ban ===
        [StringLength(200)]
        public string DepartmentId { get; set; }

        // Navigation
        [ForeignKey("UserId")]
        public virtual Users User { get; set; }

        /*    [ForeignKey("UserAppId")]
            public virtual User_App? UserApp { get; set; }   */      // bảng User_App của bạn

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        public virtual ICollection<Receipt> Receipts { get; set; } = [];
    }
}