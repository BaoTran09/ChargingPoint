 using ChargingPoint.Models;
/// ViewModel cho hiển thị danh sách sessions

namespace ChargingPoint.ViewModels
{
    public class ChargingSessionListViewModel
    {
        public List<ChargingSession> Sessions { get; set; } = new List<ChargingSession>();

        public string FilterStatus { get; set; }

        public long? StationId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
