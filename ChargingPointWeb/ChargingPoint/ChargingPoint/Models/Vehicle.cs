using ChargingPoint.DB;
using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.Models
{
    public class Vehicle
    {
        [Key]
        public long VehicleId { get; set; } // Mã duy nhất cho xe
        public long CustomerId { get; set; } // ID khách hàng
        public string VehicleType { get; set; } // Loại xe (xe máy, ô tô)
        public string Model { get; set; } // Mẫu xe (VF8, Klara S)
        public DateTime? ProductionDate { get; set; } // Ngày sản xuất
        public string BatteryType { get; set; } // Loại pin (LFP, NMC, catl, sdi)
        public string Version { get; set; } // Phiên bản (ECO, PLUS,..)
        public decimal? BatteryGrossKWh { get; set; } // Dung lượng pin tổng
        public decimal? BatteryUsableKWh { get; set; } // Dung lượng khả dụng
        public bool AcChargingSupport { get; set; } = true; // Hỗ trợ AC
        public bool DcChargingSupport { get; set; } = true; // Hỗ trợ DC
        public decimal? MaxAcChargeKW { get; set; } // Công suất AC max
        public decimal? MaxDcChargeKW { get; set; } // Công suất DC max
        public string LicensePlate { get; set; } // Biển số
        public string VIN { get; set; } // Số khung
        public string  Manufacturer { get; set; }
        public Customer Customer { get; set; } // Thêm điều hướng đến Customer
        public ICollection<ChargingSession> ChargingSessions { get; set; } = new List<ChargingSession>();

    }
}
