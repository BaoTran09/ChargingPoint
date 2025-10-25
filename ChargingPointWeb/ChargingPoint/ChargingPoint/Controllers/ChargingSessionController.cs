using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChargingPoint.DB;
using ChargingPoint.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChargingPoint.Controllers
{
    public class ChargingSessionController : Controller
    {
        private readonly StoreDBContext _context;

        public ChargingSessionController(StoreDBContext context)
        {
            _context = context;
        }

        // GET: Danh sách tất cả phiên sạc
        public async Task<IActionResult> Index()
        {
            var sessions = await _context.ChargingSession
                .Include(s => s.Connector)
                    .ThenInclude(c => c.Charger)
                        .ThenInclude(ch => ch.Station)
                .Include(s => s.Vehicle)
                    .ThenInclude(v => v.Customer)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        // GET: Chi tiết phiên sạc
        public async Task<IActionResult> Detail(long? id, long? stationId)
        {
            if (id.HasValue)
            {
                // Xem chi tiết 1 phiên sạc cụ thể - Truyền trực tiếp ChargingSession vào View
                var session = await _context.ChargingSession
                    .Include(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                            .ThenInclude(ch => ch.Station)
                    .Include(s => s.Vehicle)
                        .ThenInclude(v => v.Customer)
                    .FirstOrDefaultAsync(s => s.SessionId == id);

                if (session == null) return NotFound();

                return View(session); // Truyền trực tiếp model ChargingSession
            }
            else if (stationId.HasValue)
            {
                // Hiển thị danh sách phiên sạc của station
                var connectorIds = await _context.Connector
                    .Where(c => c.Charger.StationId == stationId)
                    .Select(c => c.ConnectorId)
                    .ToListAsync();

                var sessions = await _context.ChargingSession
                    .Include(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                            .ThenInclude(ch => ch.Station)
                    .Include(s => s.Vehicle)
                        .ThenInclude(v => v.Customer)
                    .Where(s => connectorIds.Contains(s.ConnectorId))
                    .OrderByDescending(s => s.StartTime)
                    .ToListAsync();

                ViewBag.StationId = stationId;
                return View("Index", sessions);
            }

            return NotFound();
        }

        // POST: Khởi tạo phiên sạc mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartCharging(long connectorId, long vehicleId, decimal startSOC, decimal targetSOC)
        {
            try
            {
                var vehicle = await _context.Vehicle.FindAsync(vehicleId);
                var connector = await _context.Connector
                    .Include(c => c.Charger)
                    .FirstOrDefaultAsync(c => c.ConnectorId == connectorId);

                if (vehicle == null || connector == null)
                {
                    TempData["Error"] = "Không tìm thấy xe hoặc connector";
                    return RedirectToAction("Index");
                }

                // Khởi tạo session
                var session = new ChargingSession
                {
                    ConnectorId = connectorId,
                    VehicleId = vehicleId,
                    StartTime = DateTime.Now,
                    StartSOC = startSOC,
                    TargetSOC = targetSOC,
                    Status = "Pending",
                    LastUpdated = DateTime.Now
                };

                // ĐIỀU KIỆN 1: Kiểm tra Manufacturer VinFast
                bool isVinFast = vehicle.Manufacturer?.Equals("VinFast", StringComparison.OrdinalIgnoreCase) == true;

                if (!isVinFast)
                {
                    session.Status = "Requiring";
                    _context.ChargingSession.Add(session);
                    await _context.SaveChangesAsync();

                    TempData["Warning"] = "Xe không phải VinFast - Yêu cầu phê duyệt";
                    return RedirectToAction("Detail", new { id = session.SessionId });
                }

                // ĐIỀU KIỆN 2: Kiểm tra tương thích loại sạc
                bool isChargerAC = connector.Charger.ChargerType?.Equals("AC", StringComparison.OrdinalIgnoreCase) == true;
                bool isChargerDC = connector.Charger.ChargerType?.Equals("DC", StringComparison.OrdinalIgnoreCase) == true;

                bool isCompatible = (isChargerAC && vehicle.AcChargingSupport) ||
                                   (isChargerDC && vehicle.DcChargingSupport);

                if (!isCompatible)
                {
                    session.Status = "Requiring";
                    _context.ChargingSession.Add(session);
                    await _context.SaveChangesAsync();

                    TempData["Warning"] = "Xe không hỗ trợ loại sạc này - Yêu cầu phê duyệt";
                    return RedirectToAction("Detail", new { id = session.SessionId });
                }

                // Xe VinFast và tương thích -> Bắt đầu sạc
                session.Status = "Charging";

                // ĐIỀU KIỆN 3: Tính Công suất X
                decimal vehicleMaxPower = isChargerAC
                    ? (vehicle.MaxAcChargeKW ?? 0)
                    : (vehicle.MaxDcChargeKW ?? 0);
                decimal chargerMaxPower = connector.Charger.MaxPowerKW ?? 0;

                // Lấy công suất nhỏ hơn
                decimal powerX = Math.Min(vehicleMaxPower, chargerMaxPower);

                if (powerX <= 0)
                {
                    TempData["Error"] = "Không xác định được công suất sạc";
                    return RedirectToAction("Index");
                }

                // CÔNG THỨC TÍNH TOÁN
                decimal batteryGross = vehicle.BatteryGrossKWh ?? 0;

                // 1. Dung lượng pin hiện tại (kWh): P = StartSOC * (BatteryGrossKWh / 100)
                decimal currentCapacity = (startSOC / 100) * batteryGross;
                session.MeterStartKWh = currentCapacity;

                // 2. Dung lượng cần sạc (kWh): U = [(TargetSOC - StartSOC) / 100] * BatteryGrossKWh
                decimal requiredCapacity = ((targetSOC - startSOC) / 100) * batteryGross;

                // 3. Thời gian sạc (giây): T = (U / PowerX) * 3600
                decimal timeSeconds = (requiredCapacity / powerX) * 3600;

                // 4. Thời gian ước tính: ExpectTime = StartTime + T
                session.ExpectTime = session.StartTime?.AddSeconds((double)timeSeconds);

                // Cập nhật connector status
                connector.Status = "InUse";

                _context.ChargingSession.Add(session);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Bắt đầu sạc thành công! Dự kiến xong lúc {session.ExpectTime:HH:mm:ss}";
                return RedirectToAction("Detail", new { id = session.SessionId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // API: Cập nhật progress realtime
        [HttpGet]
        public async Task<IActionResult> UpdateProgress(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Vehicle)
                    .Include(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                    return Json(new { success = false, message = "Session not found" });

                // Chỉ update nếu đang charging
                if (session.Status != "Charging")
                    return Json(new
                    {
                        success = false,
                        message = "Session không đang charging",
                        currentSOC = (double)(session.EndSOC ?? session.StartSOC ?? 0)
                    });

                // Tính SOC hiện tại dựa trên thời gian
                if (!session.StartTime.HasValue || !session.ExpectTime.HasValue)
                    return Json(new { success = false, message = "Invalid session time" });

                TimeSpan elapsed = DateTime.Now - session.StartTime.Value;
                double elapsedSeconds = elapsed.TotalSeconds;

                TimeSpan totalDuration = session.ExpectTime.Value - session.StartTime.Value;
                double totalSeconds = totalDuration.TotalSeconds;

                // Tính % hoàn thành
                double progressRatio = Math.Min(elapsedSeconds / totalSeconds, 1.0);

                // Tính SOC hiện tại
                double socRange = (double)(session.TargetSOC - session.StartSOC ?? 0);
                double currentSOC = (double)(session.StartSOC ?? 0) + (socRange * progressRatio);
                currentSOC = Math.Round(currentSOC, 2);

                // Kiểm tra đã đạt target chưa
                bool isCompleted = currentSOC >= (double)(session.TargetSOC ?? 80);

                if (isCompleted && session.PowerOffTime == null)
                {
                    // Sạc xong - ngắt điện
                    session.PowerOffTime = DateTime.Now;
                    session.EndSOC = session.TargetSOC;
                    session.Status = "PoweredOff";

                    // Tính năng lượng đã nạp
                    decimal energyDelivered = ((session.TargetSOC ?? 80) - (session.StartSOC ?? 0)) / 100 *
                                             (session.Vehicle.BatteryGrossKWh ?? 0);
                    session.MeterStopKWh = (session.MeterStartKWh ?? 0) + energyDelivered;

                    session.LastUpdated = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        isCompleted = true,
                        currentSOC = (double)(session.TargetSOC ?? 80),
                        progressPercent = 100.0,
                        message = "Sạc hoàn tất! Vui lòng tháo cáp trong 10 phút"
                    });
                }

                // Cập nhật progress
                session.EndSOC = (decimal)currentSOC;
                session.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    isCompleted = false,
                    currentSOC = currentSOC,
                    progressPercent = progressRatio * 100,
                    message = $"Đang sạc... {currentSOC:F1}%"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Phê duyệt session
        [HttpPost]
        public async Task<IActionResult> AllowCharging(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Vehicle)
                    .Include(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null || session.Status != "Requiring")
                    return Json(new { success = false, message = "Invalid session" });

                // Tính toán lại thời gian và công suất
                var vehicle = session.Vehicle;
                var connector = session.Connector;

                bool isChargerAC = connector.Charger.ChargerType?.Equals("AC", StringComparison.OrdinalIgnoreCase) == true;
                decimal vehicleMaxPower = isChargerAC
                    ? (vehicle.MaxAcChargeKW ?? 0)
                    : (vehicle.MaxDcChargeKW ?? 0);
                decimal chargerMaxPower = connector.Charger.MaxPowerKW ?? 0;
                decimal powerX = Math.Min(vehicleMaxPower, chargerMaxPower);

                decimal batteryGross = vehicle.BatteryGrossKWh ?? 0;
                decimal currentCapacity = ((session.StartSOC ?? 0) / 100) * batteryGross;
                session.MeterStartKWh = currentCapacity;

                decimal requiredCapacity = (((session.TargetSOC ?? 80) - (session.StartSOC ?? 0)) / 100) * batteryGross;
                decimal timeSeconds = (requiredCapacity / powerX) * 3600;

                session.Status = "Charging";
                session.StartTime = DateTime.Now;
                session.ExpectTime = DateTime.Now.AddSeconds((double)timeSeconds);
                session.LastUpdated = DateTime.Now;

                // Update connector
                connector.Status = "InUse";

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã phê duyệt và bắt đầu sạc" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Từ chối session
        [HttpPost]
        public async Task<IActionResult> RejectCharging(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Connector)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                    return Json(new { success = false, message = "Session not found" });

                session.Status = "Rejected";
                session.EndTime = DateTime.Now;
                session.LastUpdated = DateTime.Now;

                // Free connector
                if (session.Connector != null)
                {
                    session.Connector.Status = "Available";
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã từ chối phiên sạc" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Ngắt điện khẩn cấp
        [HttpPost]
        public async Task<IActionResult> StopCharging(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Vehicle)
                    .Include(s => s.Connector)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                    return Json(new { success = false, message = "Session not found" });

                session.PowerOffTime = DateTime.Now;
                session.EndSOC = session.EndSOC ?? session.StartSOC;
                session.Status = "Stopped";

                // Tính năng lượng đã nạp
                if (session.EndSOC.HasValue && session.StartSOC.HasValue && session.Vehicle != null)
                {
                    decimal energyDelivered = ((session.EndSOC.Value - session.StartSOC.Value) / 100) *
                                             (session.Vehicle.BatteryGrossKWh ?? 0);
                    session.MeterStopKWh = (session.MeterStartKWh ?? 0) + energyDelivered;
                }

                session.LastUpdated = DateTime.Now;

                // Free connector
                if (session.Connector != null)
                {
                    session.Connector.Status = "Available";
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã ngắt điện" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Hoàn tất session (khách đã tháo cáp)
        [HttpPost]
        public async Task<IActionResult> CompleteSession(long sessionId)
        {
            try
            {
                var session = await _context.ChargingSession
                    .Include(s => s.Connector)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                if (session == null)
                    return Json(new { success = false, message = "Session not found" });

                session.EndTime = DateTime.Now;
                session.Status = "Completed";
                session.LastUpdated = DateTime.Now;

                // Free connector
                if (session.Connector != null)
                {
                    session.Connector.Status = "Available";
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã hoàn tất phiên sạc" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    
    }
}