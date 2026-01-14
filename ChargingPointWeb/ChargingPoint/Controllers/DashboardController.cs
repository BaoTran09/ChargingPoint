/*
 * using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChargingPoint.DB;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using ChargingPoint.Models;
using ChargingPoint.ViewModels;
using System.Globalization;

namespace ChargingPoint.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class DashboardController : Controller
    {
        private readonly StoreDBContext _context;

        public DashboardController(StoreDBContext context)
        {
            _context = context;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                // Thống kê tổng quan
                TotalStations = await _context.Station.CountAsync(s => s.IsActive),
                TotalChargers = await _context.Charger.CountAsync(),
                TotalCustomers = await _context.Customer.CountAsync(),
                TotalRevenue = await _context.Invoice
                    .Where(i => i.Status == "Paid" && i.PaidAt.HasValue)
                    .SumAsync(i => i.Total ?? 0),

                // Thống kê phiên sạc
                ActiveSessions = await _context.ChargingSession
                    .CountAsync(s => s.Status == "Charging"),
                CompletedSessionsToday = await _context.ChargingSession
                    .CountAsync(s => s.Status == "Completed" &&
                                s.EndTime.HasValue &&
                                s.EndTime.Value.Date == DateTime.Today),
                TotalEnergyDeliveredToday = await _context.ChargingSession
                    .Where(s => s.EndTime.HasValue && s.EndTime.Value.Date == DateTime.Today)
                    .SumAsync(s => (s.MeterStopKWh ?? 0) - (s.MeterStartKWh ?? 0)),

                // Doanh thu theo tháng (12 tháng gần nhất)
                MonthlyRevenue = await GetMonthlyRevenue(),

                // Top 5 trạm sạc hoạt động nhiều nhất
                TopStations = await GetTopStations(),

                // Phân bố loại sạc
                ChargerTypeDistribution = await GetChargerTypeDistribution(),

                // Trạng thái connector
                ConnectorStatusSummary = await GetConnectorStatus(),

                // Hóa đơn chưa thanh toán
                PendingInvoices = await _context.Invoice
                    .Where(i => i.Status == "Pending")
                    .CountAsync(),
                PendingInvoicesAmount = await _context.Invoice
                    .Where(i => i.Status == "Pending")
                    .SumAsync(i => i.Total ?? 0),

                // Recent activities
                RecentSessions = await _context.ChargingSession
                    .Include(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                        .ThenInclude(ch => ch.Station)
                    .OrderByDescending(s => s.StartTime)
                    .Take(10)
                    .ToListAsync()
            };

            return View(model);
        }

        // API endpoints cho dashboard charts
        [HttpGet]
        public async Task<IActionResult> GetRevenueChartData(string period = "month")
        {
            var data = period switch
            {
                "day" => await GetDailyRevenue(),
                "week" => await GetWeeklyRevenue(),
                "year" => await GetYearlyRevenue(),
                _ => await GetMonthlyRevenue()
            };

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetSessionChartData()
        {
            var data = await _context.ChargingSession
                .Where(s => s.EndTime.HasValue && s.EndTime.Value >= DateTime.Today.AddDays(-30))
                .GroupBy(s => s.EndTime.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count(),
                    TotalEnergy = g.Sum(s => (s.MeterStopKWh ?? 0) - (s.MeterStartKWh ?? 0))
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetStationPerformance()
        {
            var data = await _context.Station
                .Where(s => s.IsActive)
                .Select(s => new
                {
                    StationName = s.Name,
                    TotalSessions = s.Chargers
                        .SelectMany(c => c.Connectors)
                        .SelectMany(con => con.ChargingSessions)
                        .Count(),
                    TotalRevenue = s.Chargers
                        .SelectMany(c => c.Connectors)
                        .SelectMany(con => con.ChargingSessions)
                        .SelectMany(ses => ses.Invoice)
                        .Where(inv => inv.Status == "Paid")
                        .Sum(inv => inv.Total ?? 0)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(10)
                .ToListAsync();

            return Json(data);
        }

        // Helper methods
        private async Task<object> GetMonthlyRevenue()
        {
            var data = await _context.Invoice
                .Where(i => i.Status == "Paid" && i.PaidAt.HasValue && i.PaidAt.Value >= DateTime.Today.AddMonths(-12))
                .GroupBy(i => new { i.PaidAt.Value.Year, i.PaidAt.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(i => i.Total ?? 0),
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            return data.Select(x => new
            {
                Month = $"{x.Year}-{x.Month:00}",
                x.Revenue,
                x.Count
            }).ToList();
        }

        private async Task<object> GetDailyRevenue()
        {
            var data = await _context.Invoice
                .Where(i => i.Status == "Paid" && i.PaidAt.HasValue && i.PaidAt.Value >= DateTime.Today.AddDays(-30))
                .GroupBy(i => i.PaidAt.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(i => i.Total ?? 0),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return data.Select(x => new
            {
                x.Date,
                DateLabel = x.Date.ToString("dd/MM"),
                x.Revenue,
                x.Count
            }).ToList();
        }

        private async Task<object> GetWeeklyRevenue()
        {
            var startDate = DateTime.Today.AddDays(-90);
            var data = await _context.Invoice
                .Where(i => i.Status == "Paid" && i.PaidAt.HasValue && i.PaidAt.Value >= startDate)
                .ToListAsync(); // Lấy về client trước

            var grouped = data
                .GroupBy(i => new
                {
                    Year = i.PaidAt.Value.Year,
                    Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        i.PaidAt.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                })
                .Select(g => new
                {
                    WeekLabel = $"{g.Key.Year}-W{g.Key.Week:00}",
                    Revenue = g.Sum(i => i.Total ?? 0),
                    Count = g.Count()
                })
                .OrderBy(x => x.WeekLabel)
                .ToList();

            return grouped;
        }

        private async Task<object> GetYearlyRevenue()
        {
            var data = await _context.Invoice
                .Where(i => i.Status == "Paid" && i.PaidAt.HasValue)
                .GroupBy(i => i.PaidAt.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(i => i.Total ?? 0),
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ToListAsync();

            return data; // Year đã là string rồi, không cần format
        }

        private async Task<object> GetTopStations()
        {
            var result = await _context.Station
                .Where(s => s.IsActive)
                .Select(s => new
                {
                    s.Name,
                    s.Address,
                    ChargerCount = s.Chargers.Count,
                    SessionCount = s.Chargers
                        .SelectMany(c => c.Connectors)
                        .SelectMany(con => con.ChargingSessions)
                        .Count()
                })
                .OrderByDescending(x => x.SessionCount)
                .Take(5)
                .ToListAsync();

            return result;
        }

        private async Task<object> GetChargerTypeDistribution()
        {
            var result = await _context.Charger
                .GroupBy(c => c.ChargerType ?? "Unknown")
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return result;
        }

        private async Task<object> GetConnectorStatus()
        {
            var result = await _context.Connector
                .GroupBy(c => c.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return result;
        }
    }

    // ViewModel for Dashboard
 
}*/