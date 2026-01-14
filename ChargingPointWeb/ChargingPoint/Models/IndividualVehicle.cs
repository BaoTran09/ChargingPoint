// File: Models/IndividualVehicle.cs
using ChargingPoint.DB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChargingPoint.Models
{
    public class IndividualVehicle
    {
        [Key]
        [StringLength(500)]
        public string VIN { get; set; } = null!;
        public long VehicleId { get; set; }                  // Liên kết tới mẫu xe

        public long? CustomerId { get; set; }

        [StringLength(30)]
        public string? LicensePlate { get; set; }
        public int? BatterySOC { get; set; }
        
        //SOH (State of Health - int?): Độ chai pin thực tế của xe (ví dụ 95%).
        public int? BatterySOH { get; set; }
        public DateTime? ProductionDate { get; set; } // Ngày sản xuất

        public string? Note { get; set; }
        public string? Color { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("VehicleId")]

        public Vehicle Vehicle { get; set; } = null!;
       
        [ForeignKey("CustomerId")]

        public Customer? Customer { get; set; }

        // Nếu sau này 1 chiếc xe có nhiều phiên sử dụng (charing session
        public ICollection<ChargingSession> ChargingSessions { get; set; } = new List<ChargingSession>();

    }
}