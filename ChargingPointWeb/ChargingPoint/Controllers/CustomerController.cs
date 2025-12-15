
// Controllers/CustomerController.cs
using ChargingPoint.DB;
using ChargingPoint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// [Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    private readonly StoreDBContext _context;
    private readonly UserManager<Users> _userManager;

    public CustomerController(StoreDBContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // === MY ACCOUNT ===
    public async Task<IActionResult> MyAccount()
    {
        var user = await _userManager.GetUserAsync(User);
        var customer = await _context.Customer
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        ViewBag.Customer = customer;
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> MyAccount(Users model, Customer customerModel)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        user.Email = model.Email;
        user.UserName = model.Email;

        var customer = await _context.Customer.FirstOrDefaultAsync(c => c.UserId == user.Id);
        customer.FullName = customerModel.FullName;
        customer.PhoneNumber = customerModel.PhoneNumber;
        customer.Address = customerModel.Address;
        customer.Birthday = customerModel.Birthday;

        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Cập nhật thành công!";
        return RedirectToAction("MyAccount");
    }
    // === MY CARS ===
    public async Task<IActionResult> MyCars()
    {
        var user = await _userManager.GetUserAsync(User);
        var customer = await _context.Customer
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        var cars = customer?.Vehicles?
     .Where(v => v.VehicleType == "Car")
     .ToList() ?? new List<Vehicle>();

        return View(cars);
    }

    // === MY MOTORBIKES ===
    public async Task<IActionResult> MyMotorbikes()
    {
        var user = await _userManager.GetUserAsync(User);
        var customer = await _context.Customer
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        var bikes = customer?.Vehicles?
         .Where(v => v.VehicleType == "Motorbike")
         .ToList() ?? new List<Vehicle>();

        return View(bikes);
    }
    [HttpGet]
    public async Task<IActionResult> CreateCarModal()
    {
        var user = await _userManager.GetUserAsync(User);
        var customer = await _context.Customer.FirstOrDefaultAsync(c => c.UserId == user.Id);
        var model = new Vehicle { CustomerId = customer.CustomerId, VehicleType = "Car" };
        ViewBag.Type = "Car";
        return PartialView("_VehicleModal", model);
    }

    [HttpGet]
    public async Task<IActionResult> EditCarModal(long id)
    {
        var v = await _context.Vehicle.FindAsync(id);
        if (v == null || v.VehicleType != "Car") return NotFound();
        ViewBag.Type = "Car";
        return PartialView("_VehicleModal", v);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCar(Vehicle model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Type = model.VehicleType;
            return PartialView("_VehicleModal", model);
        }

        var user = await _userManager.GetUserAsync(User);
        var customer = await _context.Customer.FirstOrDefaultAsync(c => c.UserId == user.Id);
        model.CustomerId = customer.CustomerId;
        model.CreatedAt = DateTime.UtcNow;

        _context.Vehicle.Add(model);
        await _context.SaveChangesAsync();

        var cars = await _context.Vehicle
            .Where(v => v.CustomerId == customer.CustomerId && v.VehicleType == "Car")
            .ToListAsync();

        return PartialView("_VehicleList", cars);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCar(Vehicle model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Type = model.VehicleType;
            return PartialView("_VehicleModal", model);
        }

        var vehicle = await _context.Vehicle.FindAsync(model.VehicleId);
        if (vehicle == null) return NotFound();

        // Cập nhật
        vehicle.LicensePlate = model.LicensePlate;
        vehicle.VIN = model.VIN;
        // ... các trường khác
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var cars = await _context.Vehicle
            .Where(v => v.CustomerId == vehicle.CustomerId && v.VehicleType == "Car")
            .ToListAsync();

        return PartialView("_VehicleList", cars);
    }
    // DELETE CAR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCar(long id)
    {
        var user = await _userManager.GetUserAsync(User);
        var customer = await _context.Customer.FirstOrDefaultAsync(c => c.UserId == user.Id);

        var vehicle = await _context.Vehicle.FindAsync(id);
        if (vehicle == null || vehicle.CustomerId != customer.CustomerId)
            return NotFound();

        // Check active sessions
        var hasActive = await _context.ChargingSession
            .AnyAsync(s => s.VehicleId == id && (s.Status == "Charging" || s.Status == "PoweredOff"));

        if (hasActive)
        {
            TempData["Error"] = "Không thể xóa xe đang trong phiên sạc!";
            return RedirectToAction("MyCars");
        }

        _context.Vehicle.Remove(vehicle);
        await _context.SaveChangesAsync();

        var cars = await _context.Vehicle
            .Where(v => v.CustomerId == customer.CustomerId && v.VehicleType == "Car")
            .ToListAsync();

        return PartialView("_VehicleList", cars);
    }

    // CREATE MOTORBIKE MODAL
    [HttpGet]
    public async Task<IActionResult> CreateMotorbikeModal()
    {
        var user = await _userManager.GetUserAsync(User);
        var customer = await _context.Customer.FirstOrDefaultAsync(c => c.UserId == user.Id);
        var model = new Vehicle { CustomerId = customer.CustomerId, VehicleType = "Motorbike" };
        ViewBag.Type = "Motorbike";
        return PartialView("_VehicleModal", model);
    }

    // EDIT MOTORBIKE MODAL
    [HttpGet]
    public async Task<IActionResult> EditMotorbikeModal(long id)
    {
        var v = await _context.Vehicle.FindAsync(id);
        if (v == null || v.VehicleType != "Motorbike") return NotFound();
        ViewBag.Type = "Motorbike";
        return PartialView("_VehicleModal", v);
    }

    // CREATE MOTORBIKE
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMotorbike(Vehicle model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Type = "Motorbike";
            return PartialView("_VehicleModal", model);
        }

        var user = await _userManager.GetUserAsync(User);
        var customer = await _context.Customer.FirstOrDefaultAsync(c => c.UserId == user.Id);

        model.CustomerId = customer.CustomerId;
        model.VehicleType = "Motorbike";
        _context.Vehicle.Add(model);
        await _context.SaveChangesAsync();

        var motorbikes = await _context.Vehicle
            .Where(v => v.CustomerId == customer.CustomerId && v.VehicleType == "Motorbike")
            .ToListAsync();

        return PartialView("_VehicleList", motorbikes);
    }

    // EDIT MOTORBIKE
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMotorbike(Vehicle model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Type = "Motorbike";
            return PartialView("_VehicleModal", model);
        }

        var vehicle = await _context.Vehicle.FindAsync(model.VehicleId);
        if (vehicle == null) return NotFound();

        // Update fields
        vehicle.LicensePlate = model.LicensePlate;
        vehicle.VIN = model.VIN;
        vehicle.Manufacturer = model.Manufacturer;
        vehicle.Model = model.Model;
        vehicle.Version = model.Version;
        vehicle.ProductionDate = model.ProductionDate;
        vehicle.BatteryType = model.BatteryType;
        vehicle.BatteryGrossKWh = model.BatteryGrossKWh;
        vehicle.BatteryUsableKWh = model.BatteryUsableKWh;
        vehicle.AcChargingSupport = model.AcChargingSupport;
        vehicle.DcChargingSupport = model.DcChargingSupport;
        vehicle.MaxAcChargeKW = model.MaxAcChargeKW;
        vehicle.MaxDcChargeKW = model.MaxDcChargeKW;

        await _context.SaveChangesAsync();

        var motorbikes = await _context.Vehicle
            .Where(v => v.CustomerId == vehicle.CustomerId && v.VehicleType == "Motorbike")
            .ToListAsync();

        return PartialView("_VehicleList", motorbikes);
    }

    // DELETE MOTORBIKE
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMotorbike(long id)
    {
        var user = await _userManager.GetUserAsync(User);
        var customer = await _context.Customer.FirstOrDefaultAsync(c => c.UserId == user.Id);

        var vehicle = await _context.Vehicle.FindAsync(id);
        if (vehicle == null || vehicle.CustomerId != customer.CustomerId)
            return NotFound();

        var hasActive = await _context.ChargingSession
            .AnyAsync(s => s.VehicleId == id && (s.Status == "Charging" || s.Status == "PoweredOff"));

        if (hasActive)
        {
            TempData["Error"] = "Không thể xóa xe đang trong phiên sạc!";
            return RedirectToAction("MyMotorbikes");
        }

        _context.Vehicle.Remove(vehicle);
        await _context.SaveChangesAsync();

        var motorbikes = await _context.Vehicle
            .Where(v => v.CustomerId == customer.CustomerId && v.VehicleType == "Motorbike")
            .ToListAsync();

        return PartialView("_VehicleList", motorbikes);
    }




}
