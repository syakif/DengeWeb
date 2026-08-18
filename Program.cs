using DengeWeb.Services;
using DengeWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IMailService, MailService>();

// 1. COOKIE KİMLİK DOĞRULAMA SERVİSİ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Auth/Login"; // Giriş yapmamış biri admin sayfasına girmeye çalışırsa buraya yönlendirilir
        options.LogoutPath = "/Admin/Auth/Logout"; // Çıkış yapma linki
        options.AccessDeniedPath = "/Admin/Auth/AccessDenied"; // Yetkisiz giriş denemelerinde gidilecek sayfa
        options.Cookie.Name = "DengeDefenceAdminAuth"; // Tarayıcıda tutulacak çerezin adı
        options.Cookie.HttpOnly = true; // XSS saldırılarına karşı güvenlik
        options.ExpireTimeSpan = TimeSpan.FromDays(1); // 1 gün boyunca giriş yapılı kalır
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

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
