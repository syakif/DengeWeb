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

    public HomeController(IMailService mailService, IConfiguration config, IMemoryCache cache, ApplicationDbContext context)
    {
        _mailService = mailService;
        _config = config;
        _cache = cache;
        _context = context;
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

    

    public IActionResult Contact() => View();

    [HttpPost]
public async Task<IActionResult> Contact(ContactViewModel formModel)
{
    if (ModelState.IsValid)
    {
        try
        {
            // 1. ADIM: EŞLEŞTİRME (MAPPING) VE VERİTABANINA KAYIT
            // Kullanıcıdan gelen ViewModel'i alıp, veritabanına yazılacak olan Entity sınıfına aktarıyoruz.
            var dbMessage = new ContactMessage 
            {
                // Sol taraf SQL tablosu kolonları <-- Sağ taraf kullanıcının girdiği form verileri
                AdSoyad = formModel.FullName,
                Eposta = formModel.Email,
                Telefon = formModel.Phone,
                Konu = formModel.Subject,
                Mesaj = formModel.Message,
                
                CreatedDate = DateTime.Now,
                IsRead = false
            };
            
            // Veriyi SQL'e ekle ve kaydet
            _context.ContactMessages.Add(dbMessage);
            await _context.SaveChangesAsync();

            // 2. ADIM: E-POSTA GÖNDERME
            // Mevcut MailService'in ContactViewModel beklediği için doğrudan formModel'i veriyoruz.
            await _mailService.SendEmailAsync(formModel);

            // Başarı mesajını verip sayfayı yenile
            TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi. En kısa sürede dönüş yapılacaktır.";
            return RedirectToAction("Contact");
        }
        catch (Exception ex)
        {
            // İstersen hatayı console'a yazdırabilirsin
            TempData["ErrorMessage"] = "İşlem sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
        }
    }
    
    // Model doğrulaması (Required kuralları) başarısız olursa, formu kullanıcının girdiği bilgilerle geri yükle
    return View(formModel);
}

 
}