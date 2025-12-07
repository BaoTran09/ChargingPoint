/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChargingPoint.DB;
using ChargingPoint.Models;
using ChargingPoint.ViewModels;
using System.Security.Claims;

namespace ChargingPoint.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly StoreDBContext _context;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            StoreDBContext context,
            ILogger<SettingsController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        // GET: Settings/Index - Trang chính settings
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRoles = roles;
            ViewBag.IsCustomer = roles.Contains("Customer");
            ViewBag.IsEmployee = roles.Contains("Employee") || roles.Contains("Admin");

            return View();
        }

        // ============================================
        // MY ACCOUNT (Chung cho cả Customer và Employee)
        // ============================================

        // GET: Settings/MyAccount
        public async Task<IActionResult> MyAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Customer"))
            {
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (customer == null) return NotFound();

                var viewModel = new MyAccountViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = customer.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    Birthday = customer.Birthday,
                    Address = customer.Address,
                    NationalID = customer.NationalID,
                    IsCustomer = true
                };

                return View(viewModel);
            }
            else // Employee or Admin
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserId == user.Id);

                if (employee == null) return NotFound();

                var viewModel = new MyAccountViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.EmployeeUsername,
                    FullName = employee.FullName,
                    PhoneNumber = employee.PhoneNumber,
                    Birthday = employee.Birthday,
                    Address = employee.Address,
                    JobTitle = employee.JobTitle,
                    IsCustomer = false
                };

                return View(viewModel);
            }
        }

        // POST: Settings/UpdateAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccount(MyAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("MyAccount", model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var roles = await _userManager.GetRolesAsync(user);

                // Update Email nếu thay đổi
                if (user.Email != model.Email)
                {
                    var emailExists = await _userManager.FindByEmailAsync(model.Email);
                    if (emailExists != null && emailExists.Id != user.Id)
                    {
                        ModelState.AddModelError("Email", "Email already exists");
                        return View("MyAccount", model);
                    }

                    user.Email = model.Email;
                    user.UserName = model.Email;
                    await _userManager.UpdateAsync(user);
                }

                // Update Customer or Employee info
                if (roles.Contains("Customer"))
                {
                    var customer = await _context.Customer
                        .FirstOrDefaultAsync(c => c.UserId == user.Id);

                    if (customer != null)
                    {
                        customer.FullName = model.FullName;
                        customer.PhoneNumber = model.PhoneNumber;
                        customer.Birthday = model.Birthday;
                        customer.Address = model.Address;
                        customer.NationalID = model.NationalID;
                        customer.UpdatedAt = DateTime.UtcNow;

                        await _context.SaveChangesAsync();
                    }
                }
                else // Employee
                {
                    var employee = await _context.Employees
                        .FirstOrDefaultAsync(e => e.UserId == user.Id);

                    if (employee != null)
                    {
                        employee.FullName = model.FullName;
                        employee.PhoneNumber = model.PhoneNumber;
                        employee.Birthday = model.Birthday;
                        employee.Address = model.Address;
                        employee.UpdatedAt = DateTime.UtcNow;

                        await _context.SaveChangesAsync();
                    }
                }

                TempData["Success"] = "Account updated successfully!";
                return RedirectToAction("MyAccount");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account");
                TempData["Error"] = "An error occurred while updating account";
                return View("MyAccount", model);
            }
        }

        // GET: Settings/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Settings/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction("MyAccount");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // ============================================
        // MY VEHICLES (Chỉ cho Customer)
        // ============================================

        // GET: Settings/MyCars
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyCars()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customer
                .Include(c => c.Vehicles.Where(v => v.VehicleType == "Car"))
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null) return NotFound();

            return View(customer.Vehicles.Where(v => v.VehicleType == "Car").ToList());
        }

        // GET: Settings/MyMotorbikes
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyMotorbikes()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customer
                .Include(c => c.Vehicles.Where(v => v.VehicleType == "Motorbike"))
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null) return NotFound();

            return View(customer.Vehicles.Where(v => v.VehicleType == "Motorbike").ToList());
        }

        // GET: Settings/AddVehicle
        [Authorize(Roles = "Customer")]
        public IActionResult AddVehicle(string type)
        {
            ViewBag.VehicleType = type; // "Car" or "Motorbike"
            return View();
        }

        // POST: Settings/AddVehicle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddVehicle(VehicleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.VehicleType = model.VehicleType;
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (customer == null) return NotFound();

                // Check duplicate license plate
                var existingVehicle = await _context.Vehicle
                    .FirstOrDefaultAsync(v => v.LicensePlate == model.LicensePlate);

                if (existingVehicle != null)
                {
                    ModelState.AddModelError("LicensePlate", "License plate already exists");
                    ViewBag.VehicleType = model.VehicleType;
                    return View(model);
                }

                var vehicle = new Vehicle
                {
                    CustomerId = customer.CustomerID,
                    VehicleType = model.VehicleType,
                    Model = model.Model,
                    Manufacturer = model.Manufacturer,
                    ProductionDate = model.ProductionDate,
                    BatteryType = model.BatteryType,
                    Version = model.Version,
                    BatteryGrossKWh = model.BatteryGrossKWh,
                    BatteryUsableKWh = model.BatteryUsableKWh,
                    AcChargingSupport = model.AcChargingSupport,
                    DcChargingSupport = model.DcChargingSupport,
                    MaxAcChargeKW = model.MaxAcChargeKW,
                    MaxDcChargeKW = model.MaxDcChargeKW,
                    LicensePlate = model.LicensePlate,
                    VIN = model.VIN
                };

                _context.Vehicle.Add(vehicle);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{model.VehicleType} added successfully!";

                if (model.VehicleType == "Car")
                    return RedirectToAction("MyCars");
                else
                    return RedirectToAction("MyMotorbikes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding vehicle");
                TempData["Error"] = "An error occurred while adding vehicle";
                ViewBag.VehicleType = model.VehicleType;
                return View(model);
            }
        }

        // GET: Settings/EditVehicle
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> EditVehicle(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(v => v.VehicleId == id && v.CustomerId == customer.CustomerID);

            if (vehicle == null) return NotFound();

            var viewModel = new VehicleViewModel
            {
                VehicleId = vehicle.VehicleId,
                VehicleType = vehicle.VehicleType,
                Model = vehicle.Model,
                Manufacturer = vehicle.Manufacturer,
                ProductionDate = vehicle.ProductionDate,
                BatteryType = vehicle.BatteryType,
                Version = vehicle.Version,
                BatteryGrossKWh = vehicle.BatteryGrossKWh,
                BatteryUsableKWh = vehicle.BatteryUsableKWh,
                AcChargingSupport = vehicle.AcChargingSupport,
                DcChargingSupport = vehicle.DcChargingSupport,
                MaxAcChargeKW = vehicle.MaxAcChargeKW,
                MaxDcChargeKW = vehicle.MaxDcChargeKW,
                LicensePlate = vehicle.LicensePlate,
                VIN = vehicle.VIN
            };

            return View(viewModel);
        }

        // POST: Settings/EditVehicle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> EditVehicle(VehicleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                var vehicle = await _context.Vehicle
                    .FirstOrDefaultAsync(v => v.VehicleId == model.VehicleId && v.CustomerId == customer.CustomerID);

                if (vehicle == null) return NotFound();

                vehicle.Model = model.Model;
                vehicle.Manufacturer = model.Manufacturer;
                vehicle.ProductionDate = model.ProductionDate;
                vehicle.BatteryType = model.BatteryType;
                vehicle.Version = model.Version;
                vehicle.BatteryGrossKWh = model.BatteryGrossKWh;
                vehicle.BatteryUsableKWh = model.BatteryUsableKWh;
                vehicle.AcChargingSupport = model.AcChargingSupport;
                vehicle.DcChargingSupport = model.DcChargingSupport;
                vehicle.MaxAcChargeKW = model.MaxAcChargeKW;
                vehicle.MaxDcChargeKW = model.MaxDcChargeKW;
                vehicle.LicensePlate = model.LicensePlate;
                vehicle.VIN = model.VIN;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Vehicle updated successfully!";

                if (model.VehicleType == "Car")
                    return RedirectToAction("MyCars");
                else
                    return RedirectToAction("MyMotorbikes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle");
                TempData["Error"] = "An error occurred while updating vehicle";
                return View(model);
            }
        }

        // POST: Settings/DeleteVehicle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteVehicle(long id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                var vehicle = await _context.Vehicle
                    .FirstOrDefaultAsync(v => v.VehicleId == id && v.CustomerId == customer.CustomerID);

                if (vehicle == null)
                {
                    return Json(new { success = false, message = "Vehicle not found" });
                }

                // Check if vehicle has active charging sessions
                var hasActiveSessions = await _context.ChargingSession
                    .AnyAsync(s => s.VehicleId == id &&
                                  (s.Status == "Charging" || s.Status == "PoweredOff"));

                if (hasActiveSessions)
                {
                    return Json(new { success = false, message = "Cannot delete vehicle with active charging sessions" });
                }

                string vehicleType = vehicle.VehicleType;

                _context.Vehicle.Remove(vehicle);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Vehicle deleted successfully", vehicleType = vehicleType });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vehicle");
                return Json(new { success = false, message = "An error occurred while deleting vehicle" });
            }
        }
    }
}
*/