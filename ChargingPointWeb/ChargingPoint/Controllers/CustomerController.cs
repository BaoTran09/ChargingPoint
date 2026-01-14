using ChargingPoint.DB;
using ChargingPoint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChargingPoint.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly StoreDBContext _context;
        private readonly UserManager<Users> _userManager;

        public CustomerController(StoreDBContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #region MY ACCOUNT

        public async Task<IActionResult> MyAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Sửa lỗi: Đảm bảo user.Id không null trước khi vào Lambda
            var userId = user.Id;
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            ViewBag.Customer = customer;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyAccount(Users model, Customer customerModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var userId = user.Id;
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = customerModel.PhoneNumber;

            customer.FullName = customerModel.FullName;
            customer.PhoneNumber = customerModel.PhoneNumber;
            customer.Address = customerModel.Address;
            customer.Birthday = customerModel.Birthday;
            customer.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật tài khoản!";
            }

            return RedirectToAction(nameof(MyAccount));
        }

        #endregion

        #region VEHICLE MANAGEMENT

        public async Task<IActionResult> MyCars()
        {
            return await GetVehicleView("Car");
        }

        public async Task<IActionResult> MyMotorbikes()
        {
            return await GetVehicleView("Motorbike");
        }

        private async Task<IActionResult> GetVehicleView(string type)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var userId = user.Id;
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null) return NotFound();

            var customerId = customer.CustomerId; // Lấy ID ra biến cục bộ

            var vehicles = await _context.IndividualVehicles
                .Include(iv => iv.Vehicle)
                .Where(iv => iv.CustomerId == customerId && iv.Vehicle.VehicleType == type)
                .OrderByDescending(iv => iv.CreatedAt)
                .ToListAsync();

            ViewBag.VehicleType = type;
            return View(vehicles);
        }

        [HttpGet]
        public async Task<IActionResult> CreateVehicleModal(string type)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var userId = user.Id;
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

            var vehicleModels = await _context.Vehicles
                .Where(v => v.VehicleType == type)
                .Select(v => new {
                    v.VehicleId,
                    DisplayName = v.Manufacturer + " " + v.Model + " - " + v.Version
                })
                .ToListAsync();

            ViewBag.VehicleModels = new SelectList(vehicleModels, "VehicleId", "DisplayName");
            ViewBag.Type = type;

            var model = new IndividualVehicle { CustomerId = customer?.CustomerId };
            return PartialView("_VehicleModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVehicle(IndividualVehicle model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.IndividualVehicles.AnyAsync(v => v.VIN == model.VIN))
                {
                    return BadRequest("Số VIN này đã được đăng ký trên hệ thống.");
                }

                model.CreatedAt = DateTime.UtcNow;
                model.IsActive = true;
                _context.IndividualVehicles.Add(model);
                await _context.SaveChangesAsync();

                return await GetUpdatedVehicleList(model.VehicleId, model.CustomerId);
            }
            return BadRequest("Dữ liệu không hợp lệ.");
        }

        [HttpGet]
        public async Task<IActionResult> EditVehicleModal(string vin)
        {
            var iv = await _context.IndividualVehicles
                .Include(iv => iv.Vehicle)
                .FirstOrDefaultAsync(x => x.VIN == vin);

            if (iv == null) return NotFound();

            ViewBag.Type = iv.Vehicle.VehicleType;
            return PartialView("_VehicleModal", iv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle(IndividualVehicle model)
        {
            var iv = await _context.IndividualVehicles.FindAsync(model.VIN);
            if (iv == null) return NotFound();

            iv.LicensePlate = model.LicensePlate;
            iv.Color = model.Color;
            iv.Note = model.Note;
            iv.BatterySOH = model.BatterySOH;
            iv.BatterySOC = model.BatterySOC;

            await _context.SaveChangesAsync();

            return await GetUpdatedVehicleList(iv.VehicleId, iv.CustomerId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVehicle(string vin)
        {
            var iv = await _context.IndividualVehicles
                .Include(iv => iv.Vehicle)
                .FirstOrDefaultAsync(x => x.VIN == vin);

            if (iv == null) return NotFound();

            // Sửa lỗi logic: Không dùng ?. trong AnyAsync
            var currentVin = iv.VIN;
            var hasActiveSession = await _context.ChargingSessions
                .AnyAsync(s => s.VIN == currentVin && (s.Status == "Charging" || s.Status == "Waiting"));

            if (hasActiveSession)
            {
                return Json(new { success = false, message = "Xe đang trong phiên sạc, không thể xóa!" });
            }

            long? customerId = iv.CustomerId;
            long vehicleId = iv.VehicleId;

            _context.IndividualVehicles.Remove(iv);
            await _context.SaveChangesAsync();

            return await GetUpdatedVehicleList(vehicleId, customerId);
        }

        #endregion

        private async Task<IActionResult> GetUpdatedVehicleList(long vehicleId, long? customerId)
        {
            var vehicleModel = await _context.Vehicles.FindAsync(vehicleId);
            var list = await _context.IndividualVehicles
                .Include(iv => iv.Vehicle)
                .Where(iv => iv.CustomerId == customerId && iv.Vehicle.VehicleType == vehicleModel.VehicleType)
                .OrderByDescending(iv => iv.CreatedAt)
                .ToListAsync();

            return PartialView("_VehicleList", list);
        }
    }
}