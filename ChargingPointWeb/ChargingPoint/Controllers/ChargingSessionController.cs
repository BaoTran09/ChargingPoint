using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ChargingPoint.DB;
using ChargingPoint.Models;
using ChargingPoint.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChargingPoint.Controllers
{
    // [Authorize(Roles = "Admin,Employee")]
    public class ChargingSessionController : Controller
    {
        private readonly StoreDBContext _context;
        private readonly IChargingSessionService _chargingService;
        private readonly ILogger<ChargingSessionController> _logger;

        public ChargingSessionController(
            StoreDBContext context,
            IChargingSessionService chargingService,
            ILogger<ChargingSessionController> logger)
        {
            _context = context;
            _chargingService = chargingService;
            _logger = logger;
        }

        // GET: ChargingSession/Index - Danh sách tất cả phiên sạc (Dành cho Admin/Nhân viên)
        public async Task<IActionResult> Index()
        {
            var sessions = await _context.ChargingSessions
                .Include(s => s.Connector)
                    .ThenInclude(c => c.Charger)
                        .ThenInclude(ch => ch.Station)
                .Include(s => s.IndividualVehicle) // Join bảng xe thực tế
                    .ThenInclude(iv => iv.Vehicle) // Join bảng mẫu xe
                .Include(s => s.IndividualVehicle)
                    .ThenInclude(iv => iv.Customer) // Lấy thông tin chủ xe
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        // GET: ChargingSession/Detail - Chi tiết phiên sạc
        public async Task<IActionResult> Detail(long? id, long? stationId)
        {
            if (id.HasValue)
            {
                var session = await _context.ChargingSessions
                    .Include(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                            .ThenInclude(ch => ch.Station)
                    .Include(s => s.IndividualVehicle)
                        .ThenInclude(iv => iv.Vehicle)
                    .Include(s => s.IndividualVehicle)
                        .ThenInclude(iv => iv.Customer)
                    .FirstOrDefaultAsync(s => s.SessionId == id);

                if (session == null) return NotFound();

                return View(session);
            }
            else if (stationId.HasValue)
            {
                // Lấy các phiên sạc thuộc về trạm cụ thể
                var sessions = await _context.ChargingSessions
                    .Include(s => s.Connector)
                        .ThenInclude(c => c.Charger)
                    .Include(s => s.IndividualVehicle)
                        .ThenInclude(iv => iv.Vehicle)
                    .Where(s => s.Connector.Charger.StationId == stationId)
                    .OrderByDescending(s => s.StartTime)
                    .ToListAsync();

                ViewBag.StationId = stationId;
                return View("Index", sessions);
            }

            return NotFound();
        }

        // API: Cập nhật progress realtime (Dùng chung cho dashboard Admin)
        [HttpGet]
        public async Task<IActionResult> UpdateProgress(long sessionId)
        {
            try
            {
                var progress = await _chargingService.GetChargingProgress(sessionId);
                return Json(progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy tiến trình cho session {SessionId}", sessionId);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Phê duyệt session đang ở trạng thái "Requiring"
        [HttpPost]
        public async Task<IActionResult> AllowCharging(long sessionId)
        {
            try
            {
                var result = await _chargingService.ApproveChargingSession(sessionId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi phê duyệt session {SessionId}", sessionId);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Từ chối phiên sạc (Xe hãng khác không được duyệt)
        [HttpPost]
        public async Task<IActionResult> RejectCharging(long sessionId)
        {
            try
            {
                var result = await _chargingService.RejectChargingSession(sessionId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi từ chối session {SessionId}", sessionId);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Ngắt điện khẩn cấp (Dành cho Admin/Nhân viên can thiệp)
        [HttpPost]
        public async Task<IActionResult> StopCharging(long sessionId)
        {
            try
            {
                var result = await _chargingService.StopChargingSession(sessionId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi dừng sạc session {SessionId}", sessionId);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Hoàn tất session (Ghi nhận xe đã rời đi, cáp đã tháo)
        [HttpPost]
        public async Task<IActionResult> CompleteSession(long sessionId)
        {
            try
            {
                var result = await _chargingService.CompleteChargingSession(sessionId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi hoàn tất session {SessionId}", sessionId);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Danh sách các xe đang chờ duyệt (Dành cho nhân viên trực trạm)
        public async Task<IActionResult> RequiringSessions()
        {
            var sessions = await _context.ChargingSessions
                .Include(s => s.Connector).ThenInclude(c => c.Charger)
                .Include(s => s.IndividualVehicle).ThenInclude(iv => iv.Vehicle)
                .Include(s => s.IndividualVehicle).ThenInclude(iv => iv.Customer)
                .Where(s => s.Status == "Requiring")
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        // GET: Danh sách các trụ đang có xe sạc
        public async Task<IActionResult> ActiveSessions()
        {
            var sessions = await _context.ChargingSessions
                .Include(s => s.Connector).ThenInclude(c => c.Charger)
                .Include(s => s.IndividualVehicle).ThenInclude(iv => iv.Vehicle)
                .Where(s => s.Status == "Charging")
                .OrderBy(s => s.ExpectTime)
                .ToListAsync();

            return View(sessions);
        }

        // GET: Giám sát trạm sạc theo thời gian thực (Map view hoặc Grid view)
        public async Task<IActionResult> StationMonitor(long stationId)
        {
            var station = await _context.Stations
                .Include(s => s.Chargers)
                    .ThenInclude(c => c.Connectors)
                        .ThenInclude(cn => cn.ChargingSessions
                            .Where(cs => cs.Status == "Charging" || cs.Status == "PoweredOff" || cs.Status == "Requiring"))
                            .ThenInclude(cs => cs.IndividualVehicle)
                .FirstOrDefaultAsync(s => s.StationId == stationId);

            if (station == null) return NotFound();

            return View(station);
        }
    }
}