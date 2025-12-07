using System.ComponentModel.DataAnnotations;
using ChargingPoint.DB;

namespace ChargingPoint.Models
{
    public class Connector
    {
        [Key]
        public long ConnectorId { get; set; } // Mã duy nhất cho đầu nối
        public long ChargerId { get; set; } // Liên kết với bộ sạc
        public int ConnectorIndex { get; set; } // Số thứ tự đầu nối
        public string ConnectorType { get; set; } // Loại đầu nối (CCS2, Type2, ...)
        public string Status { get; set; } = "Available"; // Trạng thái

        public Charger Charger { get; set; } // Liên kết với Charger
        public ICollection<ChargingSession> ChargingSessions { get; set; } = new List<ChargingSession>();
    }
}
