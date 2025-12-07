using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Composition;
using ChargingPoint.DB;
using Microsoft.AspNetCore.Identity;

namespace ChargingPoint.Models
{
    [Table("Employee")]
    public class Employee
    {
        [Key]
        public long EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(200)]
        public string JobTitle { get; set; }


        [Column(TypeName = "datetime2(3)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2(3)")]
        public DateTime? UpdatedAt { get; set; }

        [StringLength(200)]
        public string Status { get; set; }

        public DateTime? StartingDate { get; set; }

        public int? RoleId { get; set; }

        public int? ExperienceMonth { get; set; }

        [StringLength(450)]
        public string UserId { get; set; }

  

        [ForeignKey("UserId")]
        public virtual Users User { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        // Navigation properties
        public virtual ICollection<Receipt> Receipt { get; set; }
    }


}