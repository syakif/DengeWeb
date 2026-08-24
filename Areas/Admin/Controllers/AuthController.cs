using System.Security.Claims;
using System.Security.Cryptography;
using DengeWeb.Data;
using DengeWeb.Models;
using DengeWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DengeWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthController : Controller
{
    private const string GenericResetMessage = "E-posta adresi sistemimizde kayıtlıysa şifre sıfırlama bağlantısı gönderildi.";
    private readonly ApplicationDbContext _context;
    private readonly IMailService _mailService;
    private readonly IPasswordHasher<AdminUser> _passwordHasher;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IMailService mailService, IPasswordHasher<AdminUser> passwordHasher, IMemoryCache cache, IConfiguration configuration)
    {
        _context = context;
        _mailService = mailService;
        _passwordHasher = passwordHasher;
        _cache = cache;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);
        var email = model.Email.Trim().ToLowerInvariant();
        var key = $"admin-login:{HttpContext.Connection.RemoteIpAddress}:{email}";
        if (_cache.TryGetValue(key, out _))
        {
            ModelState.AddModelError(string.Empty, "Çok fazla deneme yapıldı. Lütfen biraz sonra tekrar deneyin.");
            return View(model);
        }

        var user = await _context.AdminUsers.SingleOrDefaultAsync(x => x.Email == email);
        var validPassword = user is not null && _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) != PasswordVerificationResult.Failed;
        if (user is null || user.LockoutEnd > DateTime.UtcNow || !validPassword)
        {
            if (user is not null)
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= 10)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.FailedAttempts = 0;
                }
                await _context.SaveChangesAsync();
            }
            _cache.Set(key, true, TimeSpan.FromSeconds(2));
            ModelState.AddModelError(string.Empty, "Hatalı e-posta veya şifre.");
            return View(model);
        }

        user.FailedAttempts = 0;
        user.LockoutEnd = null;
        await _context.SaveChangesAsync();
        var claims = new[] { new Claim(ClaimTypes.Name, user.Email), new Claim(ClaimTypes.Role, "Admin") };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        return LocalRedirect(returnUrl ?? Url.Action("Index", "Message", new { area = "Admin" })!);
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var email = model.Email.Trim().ToLowerInvariant();
            var user = await _context.AdminUsers.SingleOrDefaultAsync(x => x.Email == email);
            if (user is not null)
            {
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                user.ResetToken = Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
                user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);
                await _context.SaveChangesAsync();
                var baseUrl = _configuration["App:PublicBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
                var resetUrl = $"{baseUrl.TrimEnd('/')}{Url.Action(nameof(ResetPassword), "Auth", new { area = "Admin", token })}";
                await _mailService.SendPasswordResetEmailAsync(user.Email, resetUrl);
            }
        }
        ViewBag.Message = GenericResetMessage;
        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPassword(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction(nameof(ForgotPassword));
        }

        return View(new ResetPasswordViewModel { Token = token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Token))
        {
            ModelState.AddModelError(string.Empty, "Şifre sıfırlama bağlantısı eksik veya geçersiz.");
            return View(model);
        }

        var tokenHash = Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(model.Token)));
        var user = await _context.AdminUsers.SingleOrDefaultAsync(x => x.ResetToken == tokenHash && x.ResetTokenExpiry > DateTime.UtcNow);
        if (!ModelState.IsValid || user is null)
        {
            ModelState.AddModelError(string.Empty, "Bağlantı geçersiz veya süresi dolmuş.");
            return View(model);
        }
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        user.FailedAttempts = 0;
        user.LockoutEnd = null;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Login));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var email = User.FindFirstValue(ClaimTypes.Name);
        var user = await _context.AdminUsers.SingleOrDefaultAsync(x => x.Email == email);
        if (user is null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Mevcut şifre hatalı.");
            return View(model);
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
        await _context.SaveChangesAsync();
        TempData["Message"] = "Şifreniz başarıyla değiştirildi.";
        return RedirectToAction(nameof(ChangePassword));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}