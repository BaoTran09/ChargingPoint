using ChargingPoint.Models;
/// ViewModel cho modal yêu cầu sạc

namespace ChargingPoint.ViewModels
{
    public class RequestChargingViewModel
    {
        public long ConnectorId { get; set; }

        public Connector Connector { get; set; }

        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
