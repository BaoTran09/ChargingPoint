using ChargingPoint.DB;
using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;

namespace ChargingPoint.Models
{
    public class Charger
    {
        [Key]
        public long ChargerId { get; set; } // Mã duy nhất cho bộ sạc
        public long StationId { get; set; } // Liên kết với trạm sạc
        public string? Name { get; set; }// Tên trụ sạc
        public string? SerialNumber { get; set; } // Số sê-ri
        public string? Model { get; set; } // Model bộ sạc
        public string? ChargerType { get; set; } // 'AC' hoặc 'DC'
        public decimal? MaxPowerKW { get; set; } // Công suất tối đa
        public int? Phases { get; set; } // Số pha (cho AC)
        public decimal? OutputVoltageMin { get; set; } // Điện áp min
        public decimal? OutputVoltageMax { get; set; } // Điện áp max
        public int? PortCount { get; set; } // Số cổng
        public string? Design { get; set; } // Kiểu thiết kế
        public string?  Protections { get; set; } // Tính năng bảo vệ
        public string? FirmwareVersion { get; set; } // Phiên bản firmware
        public DateTime? InstalledAt { get; set; } // Ngày lắp
        public string? Status { get; set; } = "Online"; // Trạng thái
        public DateTime? CreatedAt { get; set; } // Ngày tạo


        public string? PicturePath { get; set; } //đường dẫn hình ảnh
        public string? UseFor { get; set; }      //Trụ sạc xe ô tô hay xe máy
        public string?Note { get; set; }      //Trụ sạc xe ô tô hay xe máy


        public Station Station { get; set; } // Liên kết với Station
        public ICollection<Connector> Connectors { get; set; } = new List<Connector>();
    }
}
