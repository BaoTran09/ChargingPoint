using ChargingPoint.Models;

namespace ChargingPoint.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStations { get; set; }
        public int TotalChargers { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ActiveSessions { get; set; }
        public int CompletedSessionsToday { get; set; }
        public decimal TotalEnergyDeliveredToday { get; set; }
        public int PendingInvoices { get; set; }
        public decimal PendingInvoicesAmount { get; set; }
        public object MonthlyRevenue { get; set; }
        public object TopStations { get; set; }
        public object ChargerTypeDistribution { get; set; }
        public object ConnectorStatusSummary { get; set; }
        public List<ChargingSession> RecentSessions { get; set; }
    }
}
