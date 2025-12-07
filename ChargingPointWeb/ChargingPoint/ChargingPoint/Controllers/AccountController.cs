using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ChargingPoint.ViewModels;
using ChargingPoint.Models;
using ChargingPoint.DB;
using ChargingPoint.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ChargingPoint.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StoreDBContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SignInManager<Users> signInManager,
            UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager,
            StoreDBContext context,
            IEmailService emailService,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login với phân biệt Customer/Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                Users user = null;

                if (model.UserType == "Employee")
                {
                    // Nhân viên: Tìm theo Username
                    if (string.IsNullOrWhiteSpace(model.Username))
                    {
                        ModelState.AddModelError("", "Username is required for employees");
                        return View(model);
                    }

                    user = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.EmployeeUsername == model.Username);

                    if (user == null)
                    {
                        ModelState.AddModelError("", "Invalid username or password");
                        return View(model);
                    }

                    // Kiểm tra user có role Employee hoặc Admin không
                    var roles = await _userManager.GetRolesAsync(user);
                    if (!roles.Contains("Employee") && !roles.Contains("Admin"))
                    {
                        ModelState.AddModelError("", "You don't have employee access");
                        return View(model);
                    }
                }
                else // Customer
                {
                    // Khách hàng: Tìm theo Email
                    user = await _userManager.FindByEmailAsync(model.Email);

                    if (user == null)
                    {
                        ModelState.AddModelError("", "Invalid email or password");
                        return View(model);
                    }
/*
                    // Kiểm tra có role Customer không
                    var roles = await _userManager.GetRolesAsync(user);
                    if (!roles.Contains("Customer"))
                    {
                        ModelState.AddModelError("", "Please login as employee");
                        return View(model);
                    }
*/
                }

                // Đăng nhập
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName, 
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {user.Email} logged in as {model.UserType}");

                    // Redirect dựa trên role
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if (userRoles.Contains("Admin") || userRoles.Contains("Employee"))
                    {
                        return RedirectToAction("Index", "Home"); // Dashboard cho nhân viên
                    }
                    else
                    {
                        return RedirectToAction("Index", "Customer"); // Trang customer
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError("", "An error occurred during login");
                return View(model);
            }
        }

        // GET: Register (Chỉ cho Customer)
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register Customer
       
        /*    public async Task<IActionResult> Register(RegisterView model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                try
                {
                    // Tạo User account
                    var user = new Users
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = false, // Cần xác thực email
                        Status = "Pending"
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        // Thêm role Customer
                        await _userManager.AddToRoleAsync(user, "Customer");

                        // Tạo Customer record
                        var customer = new Customer
                        {
                            UserId = user.Id,

                            CreatedAt = DateTime.UtcNow
                        };

                        _context.Customer.Add(customer);
                        await _context.SaveChangesAsync();

                        // Gửi email xác thực
                       // await SendVerificationEmail(user);

                        TempData["Success"] = "Registration successful! Please check your email to verify your account.";
                        return RedirectToAction("EmailSent");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during registration");
                    ModelState.AddModelError("", "An error occurred during registration");
                    ModelState.AddModelError("", ex.Message);

                }

                return View(model);
            }
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterView model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }

                var user = new Users
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = false,
                    Status = "Pending"
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }

                // Gán role Customer
                await _userManager.AddToRoleAsync(user, "Customer");

                // Gửi email với token + link bổ sung thông tin
                await SendCompleteProfileEmail(user);

                TempData["Success"] = "Đăng ký thành công! Vui lòng kiểm tra email để hoàn tất thông tin.";
                return RedirectToAction("EmailSent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đăng ký");
                ModelState.AddModelError("", "Đã xảy ra lỗi. Vui lòng thử lại.");
                return View(model);
            }
        }
        // ===== BƯỚC 4: GỬI EMAIL HOÀN TẤT HỒ SƠ =====
        private async Task SendCompleteProfileEmail(Users user)
        {
            var token = await _userManager.GenerateUserTokenAsync(
                user,
                TokenOptions.DefaultEmailProvider,
                "CompleteProfile"
            );

            var link = Url.Action(
                "CompleteProfile",
                "Account",
                new { userId = user.Id, token },
                Request.Scheme
            );

            var body = $@"
        <h3>Xin chào!</h3>
        <p>Chào mừng bạn đến với <strong>V-Green</strong>!</p>
        <p>Vui lòng hoàn tất thông tin cá nhân bằng cách nhấn vào nút bên dưới:</p>
        <p style='text-align: center;'>
            <a href='{link}' style='
                background: #28a745; 
                color: white; 
                padding: 12px 24px; 
                text-decoration: none; 
                border-radius: 5px; 
                font-weight: bold;
            '>Hoàn tất hồ sơ</a>
        </p>
        <p>Nếu bạn không đăng ký, vui lòng bỏ qua email này.</p>
    ";

            await _emailService.SendEmailAsync(
                user.Email,
                "Hoàn tất hồ sơ V-Green",
                body
            );
        }

        // ===== BƯỚC 5: TRANG HOÀN TẤT HỒ SƠ (GET) =====
        [HttpGet]
        public async Task<IActionResult> CompleteProfile(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.EmailConfirmed)
                return RedirectToAction("Login");

            var model = new CompleteProfileViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        // ===== BƯỚC 5: LƯU THÔNG TIN (POST) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(CompleteProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return RedirectToAction("Login");

            var isValid = await _userManager.VerifyUserTokenAsync(
                user,
                TokenOptions.DefaultEmailProvider,
                "CompleteProfile",
                model.Token
            );

            if (!isValid)
            {
                ModelState.AddModelError("", "Link không hợp lệ hoặc đã hết hạn.");
                return View(model);
            }

            var customer = new Customer
            {
                UserId = user.Id,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Birthday = model.Birthday,
                Address = model.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            user.EmailConfirmed = true;
            user.Status = "Active";
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Hoàn tất hồ sơ thành công! Bạn có thể đăng nhập ngay.";
            return RedirectToAction("Login");
        }

        // GET: UserManagement (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserManagement()
        {
            var employees = await _context.Employee
                .Include(e => e.User)
                .ToListAsync();

            return View(employees);
        }

        // GET: CreateEmployee (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public IActionResult CreateEmployee()
        {
            return View();
        }

        // POST: CreateEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra username đã tồn tại chưa
                var existingUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.EmployeeUsername == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(model);
                }

                // Tạo User account
                var user = new Users
                {
                    UserName = model.Email, // Email làm UserName cho Identity
                    Email = model.Email,
                    EmployeeUsername = model.Username, // Username riêng cho nhân viên
                    EmailConfirmed = true, // Admin tạo nên auto confirm
                    Status = "Active"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Thêm role Employee
                    await _userManager.AddToRoleAsync(user, "Employee");

                    // Tạo Employee record
                    var employee = new Employee
                    {
                        UserId = user.Id,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        JobTitle = model.JobTitle,
                        Status = model.Status,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Employee.Add(employee);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Employee {model.Username} created successfully!";
                    return RedirectToAction("UserManagement");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                ModelState.AddModelError("", "An error occurred while creating employee");
            }

            return View(model);
        }

        // Helper: Gửi email xác thực
        private async Task SendVerificationEmail(Users user)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmLink = Url.Action("ConfirmEmail", "Account", 
                    new { userId = user.Id, token = token }, Request.Scheme);

                var subject = "Confirm your email - V-Green";
                var body = $@"
                    <h2>Welcome to V-Green!</h2>
                    <p>Please confirm your email by clicking the link below:</p>
                    <a href='{confirmLink}'>Confirm Email</a>
                    <p>If you didn't create this account, please ignore this email.</p>
                ";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification email");
            }
        }

        // GET: ConfirmEmail
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found";
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.Status = "Active";
                await _userManager.UpdateAsync(user);

                ViewBag.SuccessMessage = "Email confirmed successfully! You can now login.";
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Error confirming email";
                return View("Error");
            }
        }

        // GET: VerifyEmail (Forgot Password)
        public IActionResult VerifyEmail()
        {
            return View();
        }

        // POST: VerifyEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email not found!");
                return View(model);
            }

            try
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ChangePassword", "Account", 
                    new { email = model.Email, token = resetToken }, Request.Scheme);

                var subject = "Reset Your Password";
                var body = $"<p>Click the link below to reset your password:</p><a href='{resetLink}'>Reset Password</a>";

                await _emailService.SendEmailAsync(model.Email, subject, body);
                return RedirectToAction("EmailSent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reset email");
                ModelState.AddModelError("", $"Failed to send reset email: {ex.Message}");
                return View(model);
            }
        }

        // GET: ChangePassword
        public IActionResult ChangePassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("VerifyEmail");
            }

            var model = new ChangePasswordView
            {
                Email = email,
                Token = token
            };
            return View(model);
        }

        // POST: ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // GET: EmailSent
        public IActionResult EmailSent()
        {
            return View();
        }

        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}