/*
 * using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChargingPoint.DB;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using ChargingPoint.Models;

namespace ChargingPoint.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ReportController : Controller
    {
        private readonly StoreDBContext _context;

        public ReportController(StoreDBContext context)
        {
            _context = context;
        }

        // GET: Report Index - Chọn loại báo cáo
        public IActionResult Index()
        {
            return View();
        }

        // ============ REVENUE REPORTS ============

        // Báo cáo doanh thu
        public IActionResult RevenueReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateRevenueReport(DateTime fromDate, DateTime toDate, string format = "view")
        {
            var data = await _context.Invoice
                .Include(i => i.Customer)
                .Include(i => i.ChargingSession)
                    .ThenInclude(s => s.Connector)
                    .ThenInclude(c => c.Charger)
                    .ThenInclude(ch => ch.Station)
                .Where(i => i.CreatedAt >= fromDate && i.CreatedAt <= toDate)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var summary = new
            {
                TotalInvoices = data.Count,
                TotalPaid = data.Count(i => i.Status == "Paid"),
                TotalPending = data.Count(i => i.Status == "Pending"),
                TotalAmount = data.Sum(i => i.Total ?? 0),
                PaidAmount = data.Where(i => i.Status == "Paid").Sum(i => i.Total ?? 0),
                PendingAmount = data.Where(i => i.Status == "Pending").Sum(i => i.Total ?? 0)
            };

            if (format == "pdf")
                return GenerateRevenuePDF(data, summary, fromDate, toDate);
            else if (format == "excel")
                return GenerateRevenueExcel(data, summary, fromDate, toDate);

            ViewBag.Summary = summary;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View("RevenueReportResult", data);
        }

        // ============ CHARGING SESSION REPORTS ============

        public IActionResult SessionReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSessionReport(DateTime fromDate, DateTime toDate, long? stationId, string format = "view")
        {
            var query = _context.ChargingSession
                .Include(s => s.Connector)
                    .ThenInclude(c => c.Charger)
                    .ThenInclude(ch => ch.Station)
                .Include(s => s.Vehicle)
                .Where(s => s.StartTime >= fromDate && s.StartTime <= toDate);

            if (stationId.HasValue)
            {
                query = query.Where(s => s.Connector.Charger.StationId == stationId.Value);
            }

            var data = await query.OrderByDescending(s => s.StartTime).ToListAsync();

            var summary = new
            {
                TotalSessions = data.Count,
                CompletedSessions = data.Count(s => s.Status == "Completed"),
                ActiveSessions = data.Count(s => s.Status == "Charging"),
                TotalEnergyDelivered = data.Sum(s => (s.MeterStopKWh ?? 0) - (s.MeterStartKWh ?? 0)),
                AverageDuration = data.Where(s => s.EndTime.HasValue)
                    .Average(s => (s.EndTime.Value - s.StartTime.Value).TotalMinutes)
            };

            if (format == "pdf")
                return GenerateSessionPDF(data, summary, fromDate, toDate);
            else if (format == "excel")
                return GenerateSessionExcel(data, summary, fromDate, toDate);

            ViewBag.Summary = summary;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Stations = await _context.Station.ToListAsync();
            return View("SessionReportResult", data);
        }

        // ============ STATION PERFORMANCE REPORTS ============

        public IActionResult StationPerformanceReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateStationPerformanceReport(DateTime fromDate, DateTime toDate, string format = "view")
        {
            var stations = await _context.Station
                .Include(s => s.Chargers)
                    .ThenInclude(c => c.Connectors)
                    .ThenInclude(con => con.ChargingSessions)
                .Where(s => s.IsActive)
                .ToListAsync();

            var data = stations.Select(s => new StationPerformanceModel
            {
                StationId = s.StationId,
                StationName = s.Name,
                Address = s.Address,
                TotalChargers = s.Chargers.Count,
                TotalConnectors = s.Chargers.SelectMany(c => c.Connectors).Count(),
                TotalSessions = s.Chargers
                    .SelectMany(c => c.Connectors)
                    .SelectMany(con => con.ChargingSessions)
                    .Count(ses => ses.StartTime >= fromDate && ses.StartTime <= toDate),
                CompletedSessions = s.Chargers
                    .SelectMany(c => c.Connectors)
                    .SelectMany(con => con.ChargingSessions)
                    .Count(ses => ses.StartTime >= fromDate && ses.StartTime <= toDate && ses.Status == "Completed"),
                TotalEnergyDelivered = s.Chargers
                    .SelectMany(c => c.Connectors)
                    .SelectMany(con => con.ChargingSessions)
                    .Where(ses => ses.StartTime >= fromDate && ses.StartTime <= toDate)
                    .Sum(ses => (ses.MeterStopKWh ?? 0) - (ses.MeterStartKWh ?? 0)),
                UtilizationRate = CalculateUtilizationRate(s, fromDate, toDate)
            }).OrderByDescending(x => x.TotalSessions).ToList();

            if (format == "pdf")
                return GenerateStationPerformancePDF(data, fromDate, toDate);
            else if (format == "excel")
                return GenerateStationPerformanceExcel(data, fromDate, toDate);

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View("StationPerformanceResult", data);
        }

        // ============ CUSTOMER REPORTS ============

        public IActionResult CustomerReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateCustomerReport(DateTime fromDate, DateTime toDate, string format = "view")
        {
            var customers = await _context.Customer
                .Include(c => c.Vehicles)
                    .ThenInclude(v => v.ChargingSessions)
                .ToListAsync();

            var data = customers.Select(c => new CustomerReportModel
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                TotalVehicles = c.Vehicles.Count,
                TotalSessions = c.Vehicles
                    .SelectMany(v => v.ChargingSessions)
                    .Count(s => s.StartTime >= fromDate && s.StartTime <= toDate),
                TotalEnergyUsed = c.Vehicles
                    .SelectMany(v => v.ChargingSessions)
                    .Where(s => s.StartTime >= fromDate && s.StartTime <= toDate)
                    .Sum(s => (s.MeterStopKWh ?? 0) - (s.MeterStartKWh ?? 0)),
                TotalSpent = _context.Invoice
                    .Where(i => i.CustomerId == c.CustomerId &&
                           i.Status == "Paid" &&
                           i.CreatedAt >= fromDate && i.CreatedAt <= toDate)
                    .Sum(i => i.Total ?? 0)
            }).OrderByDescending(x => x.TotalSpent).ToList();

            if (format == "pdf")
                return GenerateCustomerPDF(data, fromDate, toDate);
            else if (format == "excel")
                return GenerateCustomerExcel(data, fromDate, toDate);

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View("CustomerReportResult", data);
        }

        // ============ PDF GENERATION METHODS ============

        private IActionResult GenerateRevenuePDF(List<Invoice> data, object summary, DateTime fromDate, DateTime toDate)
        {
            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Title
            document.Add(new Paragraph("BÁO CÁO DOANH THU")
                .SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph($"Từ ngày: {fromDate:dd/MM/yyyy} đến {toDate:dd/MM/yyyy}")
                .SetTextAlignment(TextAlignment.CENTER));

            // Summary table (simplified - you'll need to add Vietnamese font support)
            document.Add(new Paragraph("\nTÓM TẮT:"));

            document.Close();

            return File(stream.ToArray(), "application/pdf", $"Revenue_Report_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // ============ EXCEL GENERATION METHODS ============

        private IActionResult GenerateRevenueExcel(List<Invoice> data, object summary, DateTime fromDate, DateTime toDate)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Doanh Thu");

            // Headers
            worksheet.Cell(1, 1).Value = "Mã HĐ";
            worksheet.Cell(1, 2).Value = "Ngày tạo";
            worksheet.Cell(1, 3).Value = "Khách hàng";
            worksheet.Cell(1, 4).Value = "Trạm sạc";
            worksheet.Cell(1, 5).Value = "Trạng thái";
            worksheet.Cell(1, 6).Value = "Tổng tiền";
            worksheet.Cell(1, 7).Value = "Đã thanh toán";

            // Data
            int row = 2;
            foreach (var invoice in data)
            {
                worksheet.Cell(row, 1).Value = invoice.InvoiceNumber;
                worksheet.Cell(row, 2).Value = invoice.CreatedAt;
                worksheet.Cell(row, 3).Value = invoice.Snashot_CustomerName ?? "";
                worksheet.Cell(row, 4).Value = invoice.ChargingSession?.Connector?.Charger?.Station?.Name ?? "";
                worksheet.Cell(row, 5).Value = invoice.Status;
                worksheet.Cell(row, 6).Value = invoice.Total ?? 0;
                worksheet.Cell(row, 7).Value = invoice.PaidAt?.ToString("dd/MM/yyyy") ?? "";
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Revenue_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        private IActionResult GenerateSessionExcel(List<ChargingSession> data, object summary, DateTime fromDate, DateTime toDate)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Phiên Sạc");

            // Headers
            worksheet.Cell(1, 1).Value = "Mã phiên";
            worksheet.Cell(1, 2).Value = "Trạm sạc";
            worksheet.Cell(1, 3).Value = "Bắt đầu";
            worksheet.Cell(1, 4).Value = "Kết thúc";
            worksheet.Cell(1, 5).Value = "Trạng thái";
            worksheet.Cell(1, 6).Value = "Điện năng (kWh)";
            worksheet.Cell(1, 7).Value = "Biển số xe";

            // Data
            int row = 2;
            foreach (var session in data)
            {
                worksheet.Cell(row, 1).Value = session.SessionId;
                worksheet.Cell(row, 2).Value = session.Connector?.Charger?.Station?.Name ?? "";
                worksheet.Cell(row, 3).Value = session.StartTime;
                worksheet.Cell(row, 4).Value = session.EndTime;
                worksheet.Cell(row, 5).Value = session.Status;
                worksheet.Cell(row, 6).Value = (session.MeterStopKWh ?? 0) - (session.MeterStartKWh ?? 0);
                worksheet.Cell(row, 7).Value = session.Vehicle?.LicensePlate ?? session.VIN ?? "";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Session_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        private IActionResult GenerateStationPerformanceExcel(List<StationPerformanceModel> data, DateTime fromDate, DateTime toDate)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Hiệu Suất Trạm");

            // Headers
            worksheet.Cell(1, 1).Value = "Tên trạm";
            worksheet.Cell(1, 2).Value = "Địa chỉ";
            worksheet.Cell(1, 3).Value = "Số charger";
            worksheet.Cell(1, 4).Value = "Tổng phiên";
            worksheet.Cell(1, 5).Value = "Hoàn thành";
            worksheet.Cell(1, 6).Value = "Điện năng (kWh)";
            worksheet.Cell(1, 7).Value = "Tỷ lệ sử dụng (%)";

            // Data
            int row = 2;
            foreach (var station in data)
            {
                worksheet.Cell(row, 1).Value = station.StationName;
                worksheet.Cell(row, 2).Value = station.Address;
                worksheet.Cell(row, 3).Value = station.TotalChargers;
                worksheet.Cell(row, 4).Value = station.TotalSessions;
                worksheet.Cell(row, 5).Value = station.CompletedSessions;
                worksheet.Cell(row, 6).Value = station.TotalEnergyDelivered;
                worksheet.Cell(row, 7).Value = station.UtilizationRate;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Station_Performance_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        private IActionResult GenerateCustomerExcel(List<CustomerReportModel> data, DateTime fromDate, DateTime toDate)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Khách Hàng");

            // Headers
            worksheet.Cell(1, 1).Value = "Họ tên";
            worksheet.Cell(1, 2).Value = "Email";
            worksheet.Cell(1, 3).Value = "Số điện thoại";
            worksheet.Cell(1, 4).Value = "Số xe";
            worksheet.Cell(1, 5).Value = "Tổng phiên";
            worksheet.Cell(1, 6).Value = "Điện năng (kWh)";
            worksheet.Cell(1, 7).Value = "Tổng chi tiêu";

            // Data
            int row = 2;
            foreach (var customer in data)
            {
                worksheet.Cell(row, 1).Value = customer.FullName;
                worksheet.Cell(row, 2).Value = customer.Email;
                worksheet.Cell(row, 3).Value = customer.PhoneNumber;
                worksheet.Cell(row, 4).Value = customer.TotalVehicles;
                worksheet.Cell(row, 5).Value = customer.TotalSessions;
                worksheet.Cell(row, 6).Value = customer.TotalEnergyUsed;
                worksheet.Cell(row, 7).Value = customer.TotalSpent;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Customer_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        // PDF generators (simplified placeholders)
        private IActionResult GenerateSessionPDF(List<ChargingSession> data, object summary, DateTime fromDate, DateTime toDate)
        {
            // Implementation similar to GenerateRevenuePDF
            return GenerateRevenuePDF(new List<Invoice>(), summary, fromDate, toDate);
        }

        private IActionResult GenerateStationPerformancePDF(List<StationPerformanceModel> data, DateTime fromDate, DateTime toDate)
        {
            // Implementation similar to GenerateRevenuePDF
            return GenerateRevenuePDF(new List<Invoice>(), null, fromDate, toDate);
        }

        private IActionResult GenerateCustomerPDF(List<CustomerReportModel> data, DateTime fromDate, DateTime toDate)
        {
            // Implementation similar to GenerateRevenuePDF
            return GenerateRevenuePDF(new List<Invoice>(), null, fromDate, toDate);
        }

        // Helper methods
        private decimal CalculateUtilizationRate(Station station, DateTime fromDate, DateTime toDate)
        {
            var totalConnectors = station.Chargers.SelectMany(c => c.Connectors).Count();
            if (totalConnectors == 0) return 0;

            var totalHours = (toDate - fromDate).TotalHours * totalConnectors;
            var usedHours = station.Chargers
                .SelectMany(c => c.Connectors)
                .SelectMany(con => con.ChargingSessions)
                .Where(s => s.StartTime >= fromDate && s.StartTime <= toDate && s.EndTime.HasValue)
                .Sum(s => (s.EndTime.Value - s.StartTime.Value).TotalHours);

            return (decimal)(usedHours / totalHours * 100);
        }
    }

    // Report Models
    public class StationPerformanceModel
    {
        public long StationId { get; set; }
        public string StationName { get; set; }
        public string Address { get; set; }
        public int TotalChargers { get; set; }
        public int TotalConnectors { get; set; }
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public decimal TotalEnergyDelivered { get; set; }
        public decimal UtilizationRate { get; set; }
    }

    public class CustomerReportModel
    {
        public long CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalSessions { get; set; }
        public decimal TotalEnergyUsed { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
*/