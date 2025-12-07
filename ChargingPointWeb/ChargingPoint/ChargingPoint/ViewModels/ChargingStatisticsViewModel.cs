namespace ChargingPoint.ViewModels
{
    /// ViewModel cho thống kê

    public class ChargingStatisticsViewModel
    {
        public int TotalSessions { get; set; }

        public int ActiveSessions { get; set; }

        public int CompletedSessions { get; set; }

        public decimal TotalEnergyDelivered { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}
