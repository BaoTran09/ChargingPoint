using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChargingPoint.DB;

namespace ChargingPoint.Models
{
    public class ChargingSession
    {
        [Key]
        public long SessionId { get; set; }
        public long ConnectorId { get; set; }
        public long? VehicleId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? ExpectTime { get; set; }
        public DateTime? PowerOffTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? MeterStartKWh { get; set; }
        public decimal? MeterStopKWh { get; set; }
        [NotMapped]
        public decimal? EnergyDeliveredKWh
            => MeterStopKWh.HasValue && MeterStartKWh.HasValue
                ? MeterStopKWh.Value - MeterStartKWh.Value
                : null;
        public decimal? StartSOC { get; set; }
        public decimal? EndSOC { get; set; }
        public decimal? TargetSOC { get; set; } = 80.00m; // Default 80%
        public string Status { get; set; } = "Charging"; //        // Requiring, Charging, PoweredOff, Stopped, Completed, Rejected
        public string? Notes { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public int? OverDueTime => PowerOffTime.HasValue && EndTime.HasValue
            ? (int?)DateTime.UtcNow.Subtract(PowerOffTime.Value).TotalMinutes
            : null;
        [ForeignKey("ConnectorId")]
        public virtual Connector Connector { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; }

     
    }
}