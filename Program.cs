using DengeWeb.Services;
using DengeWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using DengeWeb.Models;
using DengeWeb.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add services to the container.
builder.Services.AddControllersWithViews()
       .AddViewLocalization(); // View'larda statik çevirileri kullanabilmek için

builder.Services.AddMemoryCache(); // Caching için gerekli servis

builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddSingleton<IPasswordHasher<AdminUser>, PasswordHasher<AdminUser>>();

// 1. COOKIE KİMLİK DOĞRULAMA SERVİSİ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Auth/Login"; // Giriş yapmamış biri admin sayfasına girmeye çalışırsa buraya yönlendirilir
        options.LogoutPath = "/Admin/Auth/Logout"; // Çıkış yapma linki
        options.AccessDeniedPath = "/Admin/Auth/AccessDenied"; // Yetkisiz giriş denemelerinde gidilecek sayfa
        options.Cookie.Name = "DengeDefenceAdminAuth"; // Tarayıcıda tutulacak çerezin adı
        options.Cookie.HttpOnly = true; // XSS saldırılarına karşı güvenlik
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromDays(1); // 1 gün boyunca giriş yapılı kalır
        options.SlidingExpiration = false;
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    var adminEmail = builder.Configuration["Admin:Email"];
    var adminPassword = builder.Configuration["Admin:Password"];
    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        var normalizedEmail = adminEmail.Trim().ToLowerInvariant();
        var admin = await context.AdminUsers.OrderBy(x => x.Id).FirstOrDefaultAsync();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AdminUser>>();

        if (admin is null)
        {
            admin = new AdminUser { Email = normalizedEmail };
            admin.PasswordHash = passwordHasher.HashPassword(admin, adminPassword);
            context.AdminUsers.Add(admin);
        }
        else
        {
            admin.Email = normalizedEmail;
            admin.PasswordHash = passwordHasher.HashPassword(admin, adminPassword);
            admin.FailedAttempts = 0;
            admin.LockoutEnd = null;
        }

        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


var supportedCultures = new[] { "tr", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0]) // Varsayılan dil Türkçe
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// 2. GÜVENLİK DUVARINI AKTİF ETME
// DİKKAT: UseAuthentication satırı kesinlikle UseRouting ve UseAuthorization arasında olmalıdır!
app.UseRouting();

app.UseAuthentication(); // KİMLİK DOĞRULAMA (Eklendi)
app.UseAuthorization();

app.MapStaticAssets();

// 1. Önce Area (Admin) yönlendirmesi
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Message}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
