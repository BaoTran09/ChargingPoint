using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChargingPoint.DB;
using ChargingPoint.Models;
using ChargingPoint.ViewModels;
using ChargingPoint.Services;
using DocumentFormat.OpenXml.Bibliography;

namespace ChargingPoint.Controllers
{
    [Authorize]
    public class ChargerController : Controller
    {
       private readonly StoreDBContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly IChargingSessionService _chargingService;
        private readonly ICloudinaryImageService _imageService;
        private readonly ILogger<ChargerController> _logger;

        public ChargerController(
            StoreDBContext context,
            UserManager<Users> userManager,
            IChargingSessionService chargingService,
            ICloudinaryImageService imageService,
            ILogger<ChargerController> logger)
        {
            _context = context;
            _userManager = userManager;
            _chargingService = chargingService;
            _imageService = imageService;
            _logger = logger;
        }

        // ========================================
        // CUSTOMER: XEM DANH SÁCH TRỤ SẠC
        // ========================================

        /// <summary>
        /// GET: /Charger/Index?stationId=1&filter=Car
        /// Hiển thị danh sách trụ sạc của 1 trạm
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Index(long stationId, string? filter)
        {
            if (stationId <= 0)
                return BadRequest("Thiếu stationId");

            var station = await _context.Stations
                .Include(s => s.Chargers)
                    .ThenInclude(c => c.Connectors)
                .FirstOrDefaultAsync(s => s.StationId == stationId);

            if (station == null)
                return NotFound("Trạm sạc không tồn tại");

            var chargers = station.Chargers.AsQueryable();

            // Lọc theo loại xe
            if (!string.IsNullOrEmpty(filter))
            {
                if (filter.Equals("Car", StringComparison.OrdinalIgnoreCase))
                    chargers = chargers.Where(c => c.UseFor == "Car");
                else if (filter.Equals("Motorbike", StringComparison.OrdinalIgnoreCase))
                    chargers = chargers.Where(c => c.UseFor == "Motorbike");
            }

            var chargerList = chargers.ToList();

            // Load ảnh chính cho từng charger
            foreach (var charger in chargerList)
            {
                charger.PrimaryImageUrl = await _imageService.GetPrimaryImageUrlAsync("CHARGER", charger.ChargerId);
            }

            ViewBag.Station = station;
            ViewBag.Filter = filter;

            return View(chargerList);
        }

        // ========================================
        // CUSTOMER: YÊU CẦU SẠC
        // ========================================

        /// <summary>
        /// GET: Modal yêu cầu sạc
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> RequestCharging(long connectorId)
        {
            var connector = await _context.Connectors
                .Include(c => c.Charger).ThenInclude(ch => ch.Station)
                .FirstOrDefaultAsync(c => c.ConnectorId == connectorId);

            if (connector == null)
                return Json(new { success = false, message = "Không tìm thấy cổng sạc" });

            if (connector.Status != "Available")
                return Json(new { success = false, message = "Cổng sạc đang được sử dụng" });

            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
                return Json(new { success = false, message = "Vui lòng cập nhật thông tin khách hàng" });

            // Lấy danh sách xe THỰC TẾ của khách
            var individualVehicles = await _context.IndividualVehicles
                .Include(iv => iv.Vehicle)
                .Where(iv => iv.CustomerId == customer.CustomerId && iv.IsActive)
                .ToListAsync();

            if (!individualVehicles.Any())
                return Json(new { success = false, message = "Bạn chưa có xe. Vui lòng thêm xe trước." });

            // Lọc xe phù hợp với loại trụ sạc
            var useFor = connector.Charger.UseFor?.ToLower();
            if (useFor?.Contains("car") == true || useFor?.Contains("ô tô") == true)
            {
                individualVehicles = individualVehicles
                    .Where(v => v.Vehicle.VehicleType?.Equals("Car", StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();
            }
            else if (useFor?.Contains("motorbike") == true || useFor?.Contains("xe máy") == true)
            {
                individualVehicles = individualVehicles
                    .Where(v => v.Vehicle.VehicleType?.Equals("Motorbike", StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();
            }

            if (!individualVehicles.Any())
                return Json(new { success = false, message = $"Bạn không có xe phù hợp với trụ sạc {connector.Charger.UseFor}" });

            var vm = new RequestChargingViewModel
            {
                ConnectorId = connectorId,
                Connector = connector,
                IndividualVehicles = individualVehicles,
                StationName = connector.Charger.Station.Name,
                ChargerName = connector.Charger.Name,
                ChargerType = connector.Charger.ChargerType,
                ChargerMaxPower = connector.Charger.MaxPowerKW
            };

            return PartialView("_RequestChargingModal", vm);
        }

        /// <summary>
        /// POST: Submit yêu cầu sạc
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitChargingRequest(RequestChargingSubmitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (customer == null)
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });

                // Gọi service (không cần truyền startSOC, tự lấy từ BatterySOC)
                var result = await _chargingService.CreateChargingSession(
                    connectorId: model.ConnectorId,
                    vin: model.VIN,
                    targetSOC: model.TargetSOC,
                    customerId: customer.CustomerId
                );

                if (!result.Success)
                    return Json(new { success = false, message = result.Message });

                return Json(new
                {
                    success = true,
                    message = result.Message,
                    sessionId = result.SessionId,
                    redirectUrl = Url.Action("ChargingDetail", "Charger", new { id = result.SessionId }),
                    status = result.SessionStatus,
                    expectTime = result.ExpectTime?.ToString("HH:mm:ss dd/MM/yyyy"),
                    estimatedMinutes = result.EstimatedDurationMinutes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting charging request");
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // ========================================
        // CUSTOMER: THEO DÕI PHIÊN SẠC
        // ========================================

        /// <summary>
        /// GET: Danh sách lịch sử sạc của tôi
        /// </summary>
        public async Task<IActionResult> MyChargingSessions()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
                return NotFound("Không tìm thấy thông tin khách hàng");

            var sessions = await _context.ChargingSessions
                .Include(s => s.IndividualVehicle).ThenInclude(iv => iv.Vehicle)
                .Include(s => s.Connector).ThenInclude(c => c.Charger).ThenInclude(ch => ch.Station)
                .Where(s => s.IndividualVehicle.CustomerId == customer.CustomerId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        /// <summary>
        /// GET: Chi tiết phiên sạc realtime
        /// </summary>
        public async Task<IActionResult> ChargingDetail(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
            var isCustomer = User.IsInRole("Customer");

            var query = _context.ChargingSessions
                .Include(s => s.Connector).ThenInclude(c => c.Charger).ThenInclude(ch => ch.Station)
                .Include(s => s.IndividualVehicle).ThenInclude(iv => iv.Vehicle)
                .Include(s => s.IndividualVehicle).ThenInclude(iv => iv.Customer)
                .Where(s => s.SessionId == id);

            // Customer chỉ xem session của mình
            if (isCustomer && customer != null)
            {
                query = query.Where(s => s.IndividualVehicle.CustomerId == customer.CustomerId);
            }

            var session = await query.FirstOrDefaultAsync();
            if (session == null)
                return NotFound("Không tìm thấy phiên sạc");

            // Load invoice nếu có
            ViewBag.Invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.SessionId == session.SessionId);

            return View(session);
        }

        /// <summary>
        /// GET: API lấy tiến độ sạc realtime
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetChargingProgress(long sessionId)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            // Kiểm tra quyền truy cập
            if (User.IsInRole("Customer") && customer != null)
            {
                var hasAccess = await _context.ChargingSessions
                    .AnyAsync(s => s.SessionId == sessionId
                                && s.IndividualVehicle.CustomerId == customer.CustomerId);

                if (!hasAccess)
                    return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            var progress = await _chargingService.GetChargingProgress(sessionId);
            return Json(progress);
        }

        /// <summary>
        /// POST: Dừng sạc
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> StopCharging(long sessionId)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            // Kiểm tra quyền
            if (User.IsInRole("Customer") && customer != null)
            {
                var hasAccess = await _context.ChargingSessions
                    .AnyAsync(s => s.SessionId == sessionId
                                && s.IndividualVehicle.CustomerId == customer.CustomerId);

                if (!hasAccess)
                    return Json(new { success = false, message = "Không có quyền" });
            }

            var result = await _chargingService.StopChargingSession(sessionId);
            return Json(result);
        }

        // ========================================
        // EMPLOYEE: QUẢN LÝ TRỤ SẠC
        // ========================================

        /// <summary>
        /// GET: Create new charger (Employee only)
        /// </summary>
        /// 
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create(long stationId)
        {
            var stations = await _context.Stations.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
            var vm = new ChargerFormViewModel
            {
                StationId = stationId > 0 ? stationId : 0,
                Stations = stations
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create(ChargerFormViewModel vm)
        {
            _logger.LogInformation($"Binding test - Name: {vm?.Name}");

            // Nếu sai validation -> load lại danh sách trạm
            if (!ModelState.IsValid)
            {
                vm.Stations = await _context.Stations
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                return View(vm);
            }

            // Dùng transaction để đảm bảo an toàn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Tạo Charger
                var charger = new Charger
                {
                    StationId = vm.StationId,
                    Name = vm.Name,
                    SerialNumber = vm.SerialNumber,
                    Model = vm.Model,
                    ChargerType = vm.ChargerType,
                    MaxPowerKW = vm.MaxPowerKW,
                    Phases = vm.Phases,
                    OutputVoltageMin = vm.OutputVoltageMin,
                    OutputVoltageMax = vm.OutputVoltageMax,
                    PortCount = vm.PortCount > 0 ? vm.PortCount : 1,
                    Design = vm.Design,
                    Protections = vm.Protections,
                    FirmwareVersion = vm.FirmwareVersion,
                    InstalledAt = vm.InstalledAt,
                    Status = vm.Status ?? "Online",
                    UseFor = vm.UseFor ?? "Car",
                    Note = vm.Note,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Chargers.Add(charger);
                await _context.SaveChangesAsync(); // Lấy ID

                // 2. Tạo connectors
                for (int i = 1; i <= charger.PortCount; i++)
                {
                    var connector = new Connector
                    {
                        ChargerId = charger.ChargerId,
                        ConnectorIndex = i,
                        ConnectorType = vm.ChargerType == "DC" ? "CCS2" : "Type2",
                        Status = "Available"
                    };
                    _context.Connectors.Add(connector);
                }
                await _context.SaveChangesAsync();

                // 3. Upload hình (nếu có)
                if (vm.ImageFile != null)
                {
                    var uploadResult = await _imageService.UploadImageAsync(
                        vm.ImageFile,
                        "CHARGER",
                        charger.ChargerId,
                        isPrimary: true
                    );

                    if (!uploadResult.Success)
                    {
                        throw new Exception($"Lỗi Cloudinary: {uploadResult.Message}");
                    }
                }

                // 4. Commit
                await transaction.CommitAsync();

                TempData["Success"] = $"Đã tạo trụ sạc {charger.Name} thành công";
                return RedirectToAction("Index", new { stationId = charger.StationId });
            }
            catch (Exception ex)
            {
                // Rollback khi lỗi
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Error creating charger");
                ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");

                // Load lại dropdown trạm
                vm.Stations = await _context.Stations
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                return View(vm);
            }
        }

        /*  [Authorize(Roles = "Admin,Employee")]
          public async Task<IActionResult> Create(long stationId)
          {
              var stations = await _context.Stations.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
              var vm = new ChargerFormViewModel
              {
                  StationId = stationId > 0 ? stationId : 0,
                  Stations = stations
              };
              return View(vm);
          }

          [HttpPost]
          [ValidateAntiForgeryToken]
          [Authorize(Roles = "Admin,Employee")]

          public async Task<IActionResult> Create(ChargerFormViewModel model)
          {
              _logger.LogInformation($"Binding test - Name: {model?.Name}");
              if (!ModelState.IsValid)
              {
                  model.Stations = await _context.Stations.Where(s => s.IsActive).ToListAsync();
                  return View(model);
              }

              // SỬ DỤNG TRANSACTION ĐỂ ĐẢM BẢO AN TOÀN DỮ LIỆU
              using var transaction = await _context.Database.BeginTransactionAsync();
              try
              {
                  var charger = new Charger
                  {
                      StationId = model.StationId,
                      Name = model.Name,
                      SerialNumber = model.SerialNumber,
                      Model = model.Model,
                      ChargerType = model.ChargerType,
                      MaxPowerKW = model.MaxPowerKW,
                      Phases = model.Phases,
                      OutputVoltageMin = model.OutputVoltageMin,
                      OutputVoltageMax = model.OutputVoltageMax,
                      PortCount = model.PortCount > 0 ? model.PortCount : 1,
                      Design = model.Design,
                      Protections = model.Protections,
                      FirmwareVersion = model.FirmwareVersion,
                      InstalledAt = model.InstalledAt,
                      Status = model.Status ?? "Online",
                      UseFor = model.UseFor ?? "Car",
                      Note = model.Note,
                      CreatedAt = DateTime.UtcNow
                  };


                  _context.Chargers.Add(charger);
                  await _context.SaveChangesAsync(); // Lưu để lấy ChargerId

                  // 1. Tạo connectors tự động
                  for (int i = 1; i <= charger.PortCount; i++)
                  {
                      var connector = new Connector
                      {
                          ChargerId = charger.ChargerId,
                          ConnectorIndex = i,
                          ConnectorType = model.ChargerType == "DC" ? "CCS2" : "Type2",
                          Status = "Available"
                      };
                      _context.Connectors.Add(connector);
                  }
                  await _context.SaveChangesAsync();

                  // 2. Upload ảnh lên Cloudinary
                  if (model.ImageFile != null)
                  {
                      var uploadResult = await _imageService.UploadImageAsync(
                          model.ImageFile,
                          "CHARGER",
                          charger.ChargerId,
                          isPrimary: true
                      );

                      if (!uploadResult.Success)
                      {
                          // Nếu upload ảnh lỗi, ta chủ động ném lỗi để Rollback lại việc tạo trụ sạc
                          throw new Exception($"Lỗi Cloudinary: {uploadResult.Message}");
                      }
                  }

                  await transaction.CommitAsync(); // Hoàn tất mọi công đoạn

                  TempData["Success"] = $"Đã tạo trụ sạc {charger.Name} thành công";
                  return RedirectToAction("Index", new { stationId = charger.StationId });
              }
              catch (Exception ex)
              {
                  await transaction.RollbackAsync(); // Hủy bỏ nếu có bất kỳ lỗi nào xảy ra
                  _logger.LogError(ex, "Error creating charger");
                  ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");

                  // QUAN TRỌNG: Load lại danh sách trạm để Dropdown không bị trống
                  model.Stations = await _context.Stations.Where(s => s.IsActive).ToListAsync();
                  return View(model);
              }
          }
        */













        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(long id)
        {
            var charger = await _context.Chargers
                .Include(c => c.Station)
                .FirstOrDefaultAsync(c => c.ChargerId == id);

            if (charger == null) return NotFound();

            var vm = new ChargerFormViewModel
            {
                ChargerId = charger.ChargerId,
                StationId = charger.StationId,
                Name = charger.Name ?? "",
                SerialNumber = charger.SerialNumber,
                Model = charger.Model,
                ChargerType = charger.ChargerType ?? "AC",
                MaxPowerKW = charger.MaxPowerKW ?? 0,
                Phases = charger.Phases,
                OutputVoltageMin = charger.OutputVoltageMin,
                OutputVoltageMax = charger.OutputVoltageMax,
                PortCount = charger.PortCount ?? 1,
                Design = charger.Design,
                Protections = charger.Protections,
                FirmwareVersion = charger.FirmwareVersion,
                InstalledAt = charger.InstalledAt,
                Status = charger.Status ?? "Online",
                UseFor = charger.UseFor ?? "Car",
                Note = charger.Note,
                Stations = await _context.Stations.Where(s => s.IsActive).ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(ChargerFormViewModel model)
        {
            // Load charger gốc để giữ nguyên dữ liệu cũ
            var charger = await _context.Chargers
                .Include(c => c.Station)
                .FirstOrDefaultAsync(c => c.ChargerId == model.ChargerId);

            if (charger == null) return NotFound();

            if (!ModelState.IsValid)
            {
                // Load lại Stations + ảnh cũ khi validation fail
                model.Stations = await _context.Stations.Where(s => s.IsActive).ToListAsync();
                model.PrimaryImageUrl = await _imageService.GetPrimaryImageUrlAsync("CHARGER", model.ChargerId);
                return View(model);
            }

            try
            {
                // CHỈ CẬP NHẬT NẾU USER THAY ĐỔI
                if (!string.IsNullOrEmpty(model.Name))
                    charger.Name = model.Name;

                if (model.MaxPowerKW > 0)
                    charger.MaxPowerKW = model.MaxPowerKW;

                if (!string.IsNullOrEmpty(model.SerialNumber))
                    charger.SerialNumber = model.SerialNumber;

                if (!string.IsNullOrEmpty(model.Model))
                    charger.Model = model.Model;

                if (!string.IsNullOrEmpty(model.ChargerType))
                    charger.ChargerType = model.ChargerType;

                if (model.Phases.HasValue)
                    charger.Phases = model.Phases;

                if (model.OutputVoltageMin.HasValue)
                    charger.OutputVoltageMin = model.OutputVoltageMin;

                if (model.OutputVoltageMax.HasValue)
                    charger.OutputVoltageMax = model.OutputVoltageMax;

                if (!string.IsNullOrEmpty(model.Status))
                    charger.Status = model.Status;

                if (!string.IsNullOrEmpty(model.UseFor))
                    charger.UseFor = model.UseFor;

                if (!string.IsNullOrEmpty(model.Note))
                    charger.Note = model.Note;

                // Ảnh: Chỉ upload nếu user chọn file mới
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Xóa ảnh cũ nếu có (dùng PrimaryImageUrl để tìm)
                    var currentImages = await _context.Images
                        .Where(i => i.EntityType == "CHARGER" && i.EntityId == charger.ChargerId && i.IsPrimary)
                        .ToListAsync();

                    foreach (var img in currentImages)
                    {
                        await _imageService.DeleteImageAsync(img.Id);
                    }

                    var result = await _imageService.UploadImageAsync(model.ImageFile, "CHARGER", charger.ChargerId, true);
                    if (!result.Success)
                    {
                        ModelState.AddModelError("", result.Message);
                        model.Stations = await _context.Stations.Where(s => s.IsActive).ToListAsync();
                        model.PrimaryImageUrl = await _imageService.GetPrimaryImageUrlAsync("CHARGER", charger.ChargerId);
                        return View(model);
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật trụ sạc thành công!";
                return RedirectToAction("Index", new { stationId = charger.StationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating charger");
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                model.Stations = await _context.Stations.Where(s => s.IsActive).ToListAsync();
                model.PrimaryImageUrl = await _imageService.GetPrimaryImageUrlAsync("CHARGER", model.ChargerId);
                return View(model);
            }
        }
        /// <summary>
        /// GET: Delete confirmation
        /// </summary>
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(long id)
        {
            var charger = await _context.Chargers
                .Include(c => c.Station)
                .Include(c => c.Connectors)
                .FirstOrDefaultAsync(c => c.ChargerId == id);

            if (charger == null) return NotFound();

            // Lấy ảnh để hiển thị trong trang xác nhận xóa
            var primaryImg = await _context.Images
                .FirstOrDefaultAsync(i => i.EntityType == "CHARGER" && i.EntityId == id && i.IsPrimary);

            // Gán tạm vào NotMapped để View hiển thị
            charger.PrimaryImageUrl = primaryImg?.ImageUrl;

            return View(charger);
        }

        /// <summary>
        /// POST: Delete charger
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var charger = await _context.Chargers
                    .Include(c => c.Connectors)
                    .FirstOrDefaultAsync(c => c.ChargerId == id);

                
                if (charger == null) return NotFound();

                var stationId = charger.StationId;

                // 1. Xóa ảnh trên Cloudinary và trong bảng Image (SQL) trước
                var images = await _context.Images
                    .Where(i => i.EntityType == "CHARGER" && i.EntityId == id)
                    .ToListAsync();

                foreach (var img in images)
                {
                    // xóa file trên Cloudinary và xóa dòng trong DB Image
                    await _imageService.DeleteImageAsync(img.Id);
                }

                // 2. Xóa các Connectors liên quan (để tránh lỗi khóa ngoại)
                if (charger.Connectors != null && charger.Connectors.Any())
                {
                    _context.Connectors.RemoveRange(charger.Connectors);
                }

                // 3. Xóa chính nó
                _context.Chargers.Remove(charger);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Đã xóa trụ sạc và các dữ liệu liên quan thành công";
                return RedirectToAction("Index", new { stationId = stationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting charger");
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Delete", new { id });
            }
        }
    }
}