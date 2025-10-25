using ChargingPoint.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChargingPoint.DB
{
    public class Customer
    {
        [Key]
        public long CustomerID { get; set; }

   
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? Birthday { get; set; }

 
        public string Address { get; set; }

        
        public string NationalID { get; set; }

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; }

       
        // Quan hệ 1-n với Vehicle (1 khách có nhiều xe)
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
