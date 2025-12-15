using ChargingPoint.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChargingPoint.DB;

namespace ChargingPoint.DB
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public long CustomerId  { get; set; }

        [Required]
        [StringLength(500)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? NationalId  { get; set; }
        
        [StringLength(200)]
        public string? TaxCode { get; set; }
        
        [StringLength(200)]
        [EmailAddress]
        public string? Email { get; set; }
    
            public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Users User { get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        public virtual ICollection<Invoice> Invoice { get; set; }
        public virtual ICollection<Receipt> Receipt { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}