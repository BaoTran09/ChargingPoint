using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ChargingPoint.DB;
using ChargingPoint.Models;
using ChargingPoint.Services;
using Microsoft.Extensions.Logging;

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

// Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.SlidingExpiration = true;
});

// Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IPdfInvoiceService, PdfInvoiceService>(); // ← PDF Service
builder.Services.AddScoped<IEmailService, EmailService>(); // ← Email Service
builder.Services.AddHostedService<InvoiceBackgroundService>();
// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Service Layer
builder.Services.AddScoped<IChargingSessionService, ChargingSessionService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>(); // 

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
// 4. CẤU HÌNH HTTP PIPELINE
// ============================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

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
            Console.WriteLine($"Created role: {roleName}");
        }
    }

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
            context.Employee.Add(adminEmployee);
            await context.SaveChangesAsync();

            Console.WriteLine($"Created admin: {adminEmail}");
            Console.WriteLine($"Password: Admin@123456");
        }
        else
        {
            Console.WriteLine("Failed to create admin:");
            foreach (var error in result.Errors)
                Console.WriteLine($" - {error.Description}");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            await userManager.AddToRoleAsync(adminUser, "Admin");
        Console.WriteLine($"Admin exists: {adminEmail}");
    }
}