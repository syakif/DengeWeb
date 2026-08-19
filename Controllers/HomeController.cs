using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DengeWeb.Models;
using DengeWeb.ViewModels;
using DengeWeb.Services;
using DengeWeb.Data;
using Microsoft.Extensions.Caching.Memory;

using Microsoft.Extensions.Configuration;

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
    // 1. Önce RAM'e (Cache) bak. Eğer "SiteSettings" adında bir veri varsa direkt onu al
    if (!_cache.TryGetValue("SiteSettingsCache", out SiteSetting siteSettings))
    {
        // 2. Eğer RAM'de yoksa, SQL'den çek. (Veritabanı boşsa çökmemesi için ?? new SiteSetting() eklendi)
        siteSettings = _context.SiteSettings.FirstOrDefault() ?? new SiteSetting();

        // 3. Çektiğin bu veriyi 1 günlüğüne RAM'e kaydet
        _cache.Set("SiteSettingsCache", siteSettings, TimeSpan.FromDays(1));
    }

    // 4. Slider'da göstermek için aktif ürünleri veritabanından çek
    var products = _context.Products.Where(p => p.IsActive).ToList();

    // 5. Hem RAM'den gelen ayarları hem SQL'den gelen ürünleri tek bir Çantaya (ViewModel) koy
    var viewModel = new HomeIndexViewModel
    {
        SiteSettings = siteSettings,
        Products = products
    };

    // 6. Çantayı sayfaya gönder
    return View(viewModel); 
}

    // --- ÜRÜNLERİMİZ SAYFASI ---
        public IActionResult Products()
        {
            // Eskiden burada var urunler = new List<ProductViewModel> { ... } diyorduk.
            // ŞİMDİ DOĞRUDAN VERİTABANINDAN ÇEKİYORUZ:
            // IsActive = true olan tüm ürünleri liste halinde getir.
            var products = _context.Products.Where(p => p.IsActive).ToList();
            
            return View(products);
        }
    
    // About metodu artık dinamik veri yolluyor
    public IActionResult About()
    {
        // 1. Önce RAM'e (Cache) bak. Eğer "SiteSettings" adında bir veri varsa direkt onu al (SQL'e GİTME!)
        if (!_cache.TryGetValue("SiteSettingsCache", out SiteSetting siteSettings))
        {
            // 2. Eğer RAM'de yoksa (sunucu yeni açılmışsa), SQL'den çek.
            siteSettings = _context.SiteSettings.FirstOrDefault();

            // 3. Çektiğin bu veriyi sonsuza kadar (veya 1 günlüğüne) RAM'e kaydet ki bir sonraki kullanıcı SQL'i yormasın.
            _cache.Set("SiteSettingsCache", siteSettings, TimeSpan.FromDays(1));
        }

        return View(siteSettings); // İster DTO olsun ister devasa bir model, RAM'den geldiği için maliyet SIFIRDIR.
    }


        public IActionResult ProductDetails(int id)
        {
            // Tıklanan ID'ye göre veritabanında arama yapıyoruz
            var urun = _context.Products.FirstOrDefault(p => p.Id == id && p.IsActive);

            // Eğer ürün veritabanında bulunamazsa kullanıcıyı Ürünler sayfasına geri gönderiyoruz
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