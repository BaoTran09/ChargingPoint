// File: Models/IndividualVehicle.cs
using ChargingPoint.DB;
using System.ComponentModel.DataAnnotations;

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

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Vehicle Vehicle { get; set; } = null!;
        public Customer? Customer { get; set; }

        // Nếu sau này 1 chiếc xe có nhiều phiên sử dụng (charing session
        public ICollection<ChargingSession> ChargingSessions { get; set; } = [];
    }
}