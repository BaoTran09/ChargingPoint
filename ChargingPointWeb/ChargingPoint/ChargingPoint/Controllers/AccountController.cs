using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ChargingPoint.ViewModels;
using ChargingPoint.Models;
using ChargingPoint.DB;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ChargingPoint.Services;
using Microsoft.Extensions.Logging;

namespace ChargingPoint.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> UserManager;
        private readonly IEmailService emailService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, IEmailService emailService, ILogger<AccountController> logger)
        {
            this.signInManager = signInManager;
            this.UserManager = userManager;
            this.emailService = emailService;
            this._logger = logger;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                  
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect.");
                    return View(model);
                }
            }
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterView  model)
        {
            if (ModelState.IsValid)
            {
                Users users = new Users
                {
                    UserName = model.Email,
                    Email = model.Email,
                };
                var result = await UserManager.CreateAsync(users, model.Password);

                if ((result.Succeeded))
                {
                 return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);

                    }
                    return View(model);
                }

            }
            return View(model);
        }
      /*  public async Task<IActionResult> VerifyAccount(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }

            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found.";
                return View("VerifyEmail");
            }

            // Kiểm tra token xác minh email
            var isValid = await UserManager.VerifyUserTokenAsync(user, UserManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", token);
            if (isValid)
            {
                if (!user.EmailConfirmed) // Chỉ cập nhật nếu chưa xác minh
                {
                    user.EmailConfirmed = true;
                    await UserManager.UpdateAsync(user);
                }
                ViewBag.SuccessMessage = "Your email has been verified successfully. You can now log in.";
                return View("VerifyEmail");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid or expired verification link. Please request a new one.";
                return RedirectToAction("VerifyEmail", "Account"); // Chuyển hướng thay vì view trực tiếp
            }
        }
      */
        public IActionResult VerifyEmail()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email is not found!");
                return View(model);
            }

            try
            {
                var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ChangePassword", "Account", new { email = model.Email, token = resetToken }, Request.Scheme);
                var subject = "Reset Your Password";
                var body = $"<p>Please click the link below to reset your password:</p><a href='{resetLink}'>Reset Password</a>";
                await emailService.SendEmailAsync(model.Email, subject, body);
                return RedirectToAction("EmailSent", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to send reset email: {ex.Message}");
                return View(model);
            }
        }
        public IActionResult ChangePassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email)||string.IsNullOrEmpty(token))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            var model = new ChangePasswordView
            {
                Email = email,
                Token = token
            };
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordView model)
        {
            if (!ModelState.IsValid)
            {
               ModelState.AddModelError("", "Invalid data provided. Please check your input.");

                return View(model);

            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }
            var resetPasswordResult = await UserManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!resetPasswordResult.Succeeded)
            {
                foreach (var error in resetPasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {

                return RedirectToAction("Login", "Account");

            }
            return View(model);


        }
        [HttpGet]
        public IActionResult EmailSent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }



    }
}
