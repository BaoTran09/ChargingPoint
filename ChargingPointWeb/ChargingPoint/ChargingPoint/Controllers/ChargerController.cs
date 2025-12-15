// Controllers/ChargerController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChargingPoint.DB;
using ChargingPoint.Models;
using ChargingPoint.ViewModels;
using ChargingPoint.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ChargingPoint.Controllers
{
   // [Authorize(Roles = "Customer")]
    public class ChargerController : Controller
    {
        private readonly StoreDBContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly IChargingSessionService _chargingService;
        private readonly ILogger<ChargerController> _logger;

        public ChargerController(
            StoreDBContext context,
            UserManager<Users> userManager,
            IChargingSessionService chargingService,
            ILogger<ChargerController> logger)
        {
            _context = context;
            _userManager = userManager;
            _chargingService = chargingService;
            _logger = logger;
        }

        public async Task<IActionResult> Index([FromQuery] long stationId, [FromQuery] string? filter)
        {
            if (stationId <= 0) return BadRequest("Thiếu stationId");

            var station = await _context.Station
                .Include(s => s.Chargers)
                    .ThenInclude(c => c.Connectors)
                .FirstOrDefaultAsync(s => s.StationId == stationId);

            if (station == null) return NotFound();

            var chargers = station.Chargers.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                if (filter == "Car")
                    chargers = chargers.Where(c => c.UseFor == "Ô tô" || c.UseFor == "Car");
                else if (filter == "Motorbike")
                    chargers = chargers.Where(c => c.UseFor == "Xe máy" || c.UseFor == "Motorbike");
            }

            ViewBag.Station = station;
            ViewBag.Filter = filter;
            return View(chargers.ToList());
        }
       
        public async Task<IActionResult> RequestCharging(long connectorId)
        {
            var connector = await _context.Connector
                .Include(c => c.ConnectorId == connectorId)
                .Include(c => c.Charger)
                    .ThenInclude(ch => ch.Station)
                .FirstOrDefaultAsync();

            if (connector == null)
                return Json(new { success = false, message = "Không tìm thấy cổng sạc" });

            if (connector.Status != "Available")
                return Json(new { success = false, message = "Cổng sạc đang được sử dụng" });

            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customer
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null || !customer.Vehicles.Any())
                return Json(new { success = false, message = "Bạn chưa có xe nào. Vui lòng thêm xe trước." });

            // Lọc xe phù hợp
            var vehicles = customer.Vehicles.AsQueryable();
            var useFor = connector.Charger.UseFor?.ToLower();
            if (useFor?.Contains("ô tô") == true || useFor?.Contains("car") == true)
                vehicles = vehicles.Where(v => v.VehicleType == "Car");
            else if (useFor?.Contains("xe máy") == true || useFor?.Contains("motorbike") == true)
                vehicles = vehicles.Where(v => v.VehicleType == "Motorbike");

            var vm = new RequestChargingViewModel
            {
                ConnectorId = connectorId,
                Connector = connector,
                Vehicles = vehicles.ToList()
            };

            // QUAN TRỌNG: Trả về FULL MODAL HTML
            return PartialView("_RequestChargingModal", vm);
        }

        // POST: Charger/SubmitChargingRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitChargingRequest(RequestChargingSubmitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (customer == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });
                }

                // Validate vehicle belongs to customer
                var vehicle = await _context.Vehicle
                    .FirstOrDefaultAsync(v => v.VehicleId == model.VehicleId && v.CustomerId == customer.CustomerId);

                if (vehicle == null)
                {
                    return Json(new { success = false, message = "Xe không tồn tại hoặc không thuộc về bạn" });
                }

                // ============================================
                // GỌI SERVICE ĐỂ XỬ LÝ LOGIC CHUNG
                // ============================================
                var result = await _chargingService.CreateChargingSession(
                    connectorId: model.ConnectorId,
                    vehicleId: model.VehicleId,
                    startSOC: model.StartSOC,
                    targetSOC: model.TargetSOC
                );

                if (!result.Success)
                {
                    return Json(new { success = false, message = result.Message });
                }

                // Return response
                string message = result.SessionStatus == "Charging"
                    ? $"Đã bắt đầu sạc! Dự kiến hoàn tất lúc {result.ExpectTime:HH:mm:ss}"
                    : "Yêu cầu sạc đã được gửi. Vui lòng chờ nhân viên xác nhận.";

                string redirectUrl = Url.Action("ChargingDetail", "Charger", new { id = result.SessionId });

                return Json(new
                {
                    success = true,
                    message = message,
                    sessionId = result.SessionId,
                    redirectUrl = redirectUrl,
                    status = result.SessionStatus,
                    expectTime = result.ExpectTime?.ToString("HH:mm:ss dd/MM/yyyy")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating charging request");
                return Json(new
                {
                    success = false,
                    message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        // GET: Charger/MyChargingSessions
        public async Task<IActionResult> MyChargingSessions()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null) return NotFound();

            var sessions = await _context.ChargingSession
                .Include(s => s.Vehicle)
                .Include(s => s.Connector)
                    .ThenInclude(c => c.Charger)
                        .ThenInclude(ch => ch.Station)
                .Where(s => s.Vehicle.CustomerId == customer.CustomerId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        // GET: Charger/ChargingDetail/{id}
        public async Task<IActionResult> ChargingDetail(long id)
        {
            // 1. Phân quyền và xác định Customer
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            // Nếu không phải Customer, giả định là Admin/Employee và bỏ qua bước kiểm tra quyền sở hữu Session
            var isCustomerView = User.IsInRole("Customer");

            if (isCustomerView && customer == null) return NotFound();

            // 2. Lấy ChargingSession
            var query = _context.ChargingSession
                .Include(s => s.Connector)
                    .ThenInclude(c => c.Charger)
                        .ThenInclude(ch => ch.Station)
                .Include(s => s.Vehicle)
                    .ThenInclude(v => v.Customer)
                .Where(s => s.SessionId == id);

            // Áp dụng điều kiện bảo mật: Customer chỉ xem được Session của mình
            if (isCustomerView)
            {
                query = query.Where(s => s.Vehicle.CustomerId == customer.CustomerId);
            }

            var session = await query.FirstOrDefaultAsync();

            if (session == null) return NotFound();

            // 3. ⭐ THÊM LOGIC: Lấy Invoice và truyền vào ViewBag
            var invoice = await _context.Invoice
                .FirstOrDefaultAsync(i => i.SessionId == session.SessionId);

            ViewBag.Invoice = invoice; // Truyền Invoice object vào ViewBag

            // 4. Trả về View
            return View("ChargingDetail", session);
        }
        

        [HttpGet]
        public async Task<IActionResult> GetMyChargingProgress(long sessionId)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
                return Json(new { success = false, message = "Unauthorized" });

            // Verify session belongs to customer
            var session = await _context.ChargingSession
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.Vehicle.CustomerId == customer.CustomerId);

            if (session == null)
                return Json(new { success = false, message = "Session not found" });

            // Gọi service
            var progress = await _chargingService.GetChargingProgress(sessionId);

            return Json(progress);
        }

        // POST: Stop My Charging (Customer có thể dừng sạc của mình)
        [HttpPost]
        public async Task<IActionResult> StopMyCharging(long sessionId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (customer == null)
                    return Json(new { success = false, message = "Unauthorized" });

                // Verify session belongs to customer
                var session = await _context.ChargingSession
                    .Include(s => s.Vehicle)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.Vehicle.CustomerId == customer.CustomerId);

                if (session == null)
                    return Json(new { success = false, message = "Session not found" });

                // Gọi service
                var result = await _chargingService.StopChargingSession(sessionId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping charging session {SessionId}", sessionId);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}