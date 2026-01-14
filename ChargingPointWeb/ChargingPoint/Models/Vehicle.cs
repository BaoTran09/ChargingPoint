using ChargingPoint.DB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChargingPoint.Models
{
    public class Vehicle
    {
        [Key]
        public long VehicleId { get; set; } // Mã duy nhất cho mẫu xe
        public string? VehicleType { get; set; } // Loại xe (xe máy, ô tô)
        public string? Model { get; set; } // Mẫu xe (VF8,...)
        public string? BatteryType { get; set; } // Loại pin (LFP, NMC, catl, sdi)
        public string? Version { get; set; } // Phiên bản (ECO, PLUS,..)
        public decimal? BatteryGrossKWh { get; set; } // Dung lượng pin tổng
        public decimal? BatteryUsableKWh { get; set; } // Dung lượng khả dụng
        public bool AcChargingSupport { get; set; } = true; // Hỗ trợ AC
        public bool DcChargingSupport { get; set; } = true; // Hỗ trợ DC
        public decimal? MaxAcChargeKW { get; set; } // Công suất AC max
        public decimal? MaxDcChargeKW { get; set; } // Công suất DC max
        public string?  Manufacturer { get; set; }
        public int? NominalVoltage { get; set; }
        public string? ConnectorType { get; set; } // Loại cổng kết nối (CCS2, Type 2,...)
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
  
        public ICollection<IndividualVehicle> IndividualVehicles { get; set; } = new List<IndividualVehicle>();

    }
}
