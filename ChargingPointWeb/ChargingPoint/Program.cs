using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ChargingPoint.DB;
using ChargingPoint.Models;
using ChargingPoint.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. CẤU HÌNH DỊCH VỤ
// ============================================

// MVC
builder.Services.AddControllersWithViews();

// Database
string path = Directory.GetCurrentDirectory();
builder.Services.AddDbContext<StoreDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
            .Replace("[DataDirectory]", path)));

// Identity
builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<StoreDBContext>()
.AddDefaultTokenProviders();

// Cookie Configuration (Authentication)

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsProduction()
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.SameAsRequest;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.SlidingExpiration = true;
});

// Antiforgery Configuration
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = builder.Environment.IsProduction()
        ? "__Host-Antiforgery"
        : "Antiforgery";
    options.Cookie.SecurePolicy = builder.Environment.IsProduction()
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.HeaderName = "X-CSRF-TOKEN";
});

// Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsProduction()
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.SameAsRequest;
});

// HSTS Configuration (only for production)
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });
}

// Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//builder.Services.AddScoped<IPdfInvoiceService, PdfInvoiceService>();
builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddHostedService<InvoiceBackgroundService>();

// Service Layer
builder.Services.AddScoped<IChargingSessionService, ChargingSessionService>();
// HttpClient cho Cloudinary

builder.Services.AddHttpClient<ICloudinaryImageService, CloudinaryImageService>();

// Image Service
builder.Services.AddScoped<ICloudinaryImageService, CloudinaryImageService>();



//builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// ============================================
// 2. XÂY DỰNG APP
// ============================================

var app = builder.Build();

// CHẶN TẤT CẢ REQUEST HTTP - PHẢI Ở ĐẦU TIÊN
app.Use(async (context, next) =>
{
    // Nếu là HTTP, từ chối luôn
    if (!context.Request.IsHttps)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("HTTPS Required");
        return;
    }
    await next();
});
// ============================================
// 3. SEED ROLES & ADMIN (CHỈ CHẠY 1 LẦN)
// ============================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<Users>>();
        var context = services.GetRequiredService<StoreDBContext>();
        await SeedRolesAndAdminAsync(roleManager, userManager, context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi seed roles và admin");
    }
}

// ============================================
// 4. CẤU HÌNH MIDDLEWARE PIPELINE
// ============================================
// Middleware order is CRITICAL! Follow this exact order:

// 1. Exception Handling (must be first)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Only in production
}

// 2. Force HTTPS (block any HTTP requests)
app.Use(async (context, next) =>
{
    if (!context.Request.IsHttps)
    {
        var httpsUrl = $"https://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect(httpsUrl, permanent: true);
        return;
    }
    await next();
});

// 3. Security Headers (apply to all responses)
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    // Upgrade Insecure Requests (forces HTTPS)
    context.Response.Headers.Append("Upgrade-Insecure-Requests", "1");

    // Content Security Policy (optional but recommended)
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; upgrade-insecure-requests;");
    }

    await next();
});

// 3. Cookie Policy (before HTTPS redirection)
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = app.Environment.IsProduction()
        ? SameSiteMode.Strict
        : SameSiteMode.Lax,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = app.Environment.IsProduction()
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.SameAsRequest
});

// 4. HTTPS Redirection
app.UseHttpsRedirection();

// 5. Static Files
app.UseStaticFiles();

// 6. Routing
app.UseRouting();

// 7. CORS (if needed)
// app.UseCors();

// 8. Authentication (must be before Authorization)
app.UseAuthentication();

// 9. Authorization (must be after Authentication)
app.UseAuthorization();

// 10. Session (after Authentication/Authorization)
app.UseSession();

// 11. Endpoints (must be last)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ============================================
// 5. CHẠY ỨNG DỤNG
// ============================================

app.Run();

// ============================================
// HELPER: SEED ROLES & ADMIN
// ============================================

static async Task SeedRolesAndAdminAsync(
    RoleManager<IdentityRole> roleManager,
    UserManager<Users> userManager,
    StoreDBContext context)
{
    // 1. Tạo Roles
    string[] roles = { "Admin", "Employee", "Customer" };
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
            Console.WriteLine($"✓ Created role: {roleName}");
        }
    }
    



























    ///BỎ ĐOẠN NÀY
    // 2. Tạo Admin
    string adminEmail = "lebaotran.forwork@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new Users
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            EmployeeUsername = "admin",
            Status = "Active"
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123456");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");

            var adminEmployee = new Employee
            {
                UserId = adminUser.Id,
                FullName = "System Administrator",
                JobTitle = "Administrator",
                PhoneNumber = "0123456789",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };
            context.Employees.Add(adminEmployee);
            await context.SaveChangesAsync();

            Console.WriteLine($"✓ Created admin: {adminEmail}");
            Console.WriteLine($"  Password: Admin@123456");
        }
        else
        {
            Console.WriteLine("✗ Failed to create admin:");
            foreach (var error in result.Errors)
                Console.WriteLine($"  - {error.Description}");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        Console.WriteLine($"✓ Admin exists: {adminEmail}");
    }
    




}