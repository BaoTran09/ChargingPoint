// Controllers/EmployeeController.cs
using ChargingPoint.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // THÊM DÒNG NÀY
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Employee,Admin")]
public class EmployeeController : Controller
{
    private readonly StoreDBContext _context;
    private readonly UserManager<Users> _userManager;

    public EmployeeController(StoreDBContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> MyAccount()
    {
        var user = await _userManager.GetUserAsync(User);
        var employee = await _context.Employee
            .FirstOrDefaultAsync(e => e.UserId == user.Id);

        ViewBag.Employee = employee;
        return View(user);
    }
}