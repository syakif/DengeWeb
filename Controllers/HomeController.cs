using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DengeWeb.Models;
using DengeWeb.ViewModels;
using DengeWeb.Services;
using DengeWeb.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace DengeWeb.Controllers;

public class HomeController : Controller
{
    private readonly IMailService _mailService;
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache; // Cache servisini çağırıyoruz

    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IMailService mailService, IConfiguration config, IMemoryCache cache, ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _mailService = mailService;
        _config = config;
        _cache = cache;
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
{
    string currentLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    string cacheKey = "SiteSettings_" + currentLang; // Cache anahtarını dile özel yaptık

    if (!_cache.TryGetValue(cacheKey, out SiteSetting siteSettings))
    {
        siteSettings = _context.SiteSettings.FirstOrDefault(s => s.Language == currentLang) ?? new SiteSetting();
        _cache.Set(cacheKey, siteSettings, TimeSpan.FromDays(1));
    }

    var products = _context.Products.Where(p => p.IsActive && p.Language == currentLang).ToList();

    var viewModel = new HomeIndexViewModel { SiteSettings = siteSettings, Products = products };
    return View(viewModel);
}
    
    [Route("{culture=tr}/hakkimizda")]
    [Route("{culture=en}/about")]
    // About metodu artık dinamik veri yolluyor
    public IActionResult About()
    {

        string currentLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        string cacheKey = "SiteSettings_" + currentLang; // Cache anahtarını dile özel yaptık

        // 1. Önce RAM'e (Cache) bak. Eğer "SiteSettings" adında bir veri varsa direkt onu al (SQL'e GİTME!)
        if (!_cache.TryGetValue(cacheKey, out SiteSetting siteSettings))
        {
            // 2. Eğer RAM'de yoksa (sunucu yeni açılmışsa), SQL'den çek.
            siteSettings = _context.SiteSettings.FirstOrDefault(s => s. Language == currentLang);

            // 3. Çektiğin bu veriyi sonsuza kadar (veya 1 günlüğüne) RAM'e kaydet ki bir sonraki kullanıcı SQL'i yormasın.
            _cache.Set(cacheKey, siteSettings, TimeSpan.FromDays(1));
        }

        return View(siteSettings); // İster DTO olsun ister devasa bir model, RAM'den geldiği için maliyet SIFIRDIR.
    }

    [Route("{culture=en}/products")]
    [Route("{culture=tr}/urunlerimiz")]
    // --- ÜRÜNLERİMİZ SAYFASI ---
    public IActionResult Products()
{
        string currentLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        string cacheKey = "SiteSettings_" + currentLang; // Cache anahtarını dile özel yaptık

        
        // 1. Sitenin dilini bilmemiz gerektiği için ayarları Cache'den çağırıyoruz
        if (!_cache.TryGetValue(cacheKey, out SiteSetting siteSettings))
        {
            siteSettings = _context.SiteSettings.FirstOrDefault(s => s.Language == currentLang) ?? new SiteSetting();
            _cache.Set(cacheKey, siteSettings, TimeSpan.FromDays(1));
        }

        // 2. Hem aktif olan HEM DE sitenin mevcut diliyle eşleşen ürünleri çekiyoruz
        var products = _context.Products
                        .Where(p => p.IsActive && p.Language == siteSettings.Language)
                        .ToList();
                           
        return View(products);
    }

        [Route("{culture=tr}/urun-detay/{id}")]
        [Route("{culture=en}/product-details/{id}")]
        public IActionResult ProductDetails(int id)
        {
            // 1. Yine sitenin mevcut ayarlarını (dilini) Cache'den çağırıyoruz
            string currentLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            string cacheKey = "SiteSettings_" + currentLang;

            if (!_cache.TryGetValue(cacheKey, out SiteSetting siteSettings))
            {
                siteSettings = _context.SiteSettings.FirstOrDefault(s => s.Language == currentLang) ?? new SiteSetting();
                _cache.Set(cacheKey, siteSettings, TimeSpan.FromDays(1));
            }

            // 2. Tıklanan ID'ye sahip, aktif olan VE sitenin diline uygun olan ürünü arıyoruz
            var urun = _context.Products.FirstOrDefault(p => p.Id == id && 
                                                     p.IsActive && 
                                                     p.Language == currentLang);

            // Eğer ürün bulunamazsa veya farklı bir dile aitse, kullanıcıyı listeye geri gönder
            if (urun == null)
            {
                return RedirectToAction("Products");
            }

            return View(urun);
        }

    

    [Route("{culture=tr}/iletisim")]
    [Route("{culture=en}/contact")]
    // --- 1. GET METODU (Sayfa İlk Açıldığında Çalışır) ---
public IActionResult Contact()
{
    // O anki dili bul (tr veya en)
    string currentLang = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

    // Veritabanından geçerli dilin ayarlarını çek
    var currentSettings = _context.SiteSettings.FirstOrDefault(s => s.Language == currentLang);

    // ViewModel'i oluştur ve ayarları içine koyarak sayfaya gönder
    var viewModel = new ContactViewModel
    {
        SiteSettings = currentSettings
    };

    return View(viewModel);
}

[HttpPost]
[Route("{culture=tr}/iletisim")]
[Route("{culture=en}/contact")]
public async Task<IActionResult> Contact(ContactViewModel formModel)
{
    // MVC'nin formda olmayan SiteSettings'i doğrulamasını engelliyoruz
    ModelState.Remove("SiteSettings");

    if (ModelState.IsValid)
    {
        try
        {
            // 1. ADIM: EŞLEŞTİRME VE VERİTABANINA KAYIT
            var dbMessage = new ContactMessage 
            {
                AdSoyad = formModel.FullName,
                Eposta = formModel.Email,
                Telefon = formModel.Phone,
                Konu = formModel.Subject,
                Mesaj = formModel.Message,
                
                CreatedDate = DateTime.Now,
                IsRead = false
            };
            
            _context.ContactMessages.Add(dbMessage);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Contact form mesajı veritabanına kaydedildi. MessageId: {MessageId}", dbMessage.Id);

            // 2. ADIM: E-POSTA GÖNDERME
            await _mailService.SendEmailAsync(formModel);
            _logger.LogInformation("Contact form maili başarıyla gönderildi. MessageId: {MessageId}", dbMessage.Id);

            // DÜZELTİLEN KISIM: İsimler HTML ile tam uyumlu hale getirildi
            TempData["MailStatus"] = "success"; 
            return RedirectToAction("Contact");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Contact form işlemi başarısız oldu. ExceptionType: {ExceptionType}", ex.GetType().Name);
            TempData["MailStatus"] = "error"; 
        }
    }
    
    // Model doğrulaması (Required kuralları) başarısız olursa formu geri yükle
    string currentLang = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    formModel.SiteSettings = _context.SiteSettings.FirstOrDefault(s => s.Language == currentLang);
    
    return View(formModel);
}

 
}