using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DengeWeb.Models;
using DengeWeb.Services;
using DengeWeb.Data;

using Microsoft.Extensions.Configuration;

namespace DengeWeb.Controllers;

public class HomeController : Controller
{
    private readonly IMailService _mailService;
    private readonly IConfiguration _config;

    private readonly ApplicationDbContext _context;

    public HomeController(IMailService mailService, IConfiguration config, ApplicationDbContext context)
    {
        _mailService = mailService;
        _config = config;
        _context = context;
    }

    //public IActionResult Index() => View();

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
        var siteSettings = _context.SiteSettings.FirstOrDefault();

        return View(siteSettings); 
    }

    /*public IActionResult Products()
    {
        var products = new List<ProductViewModel>
        {
            new ProductViewModel { Id = 1, Name = "Taktik İHA X-1", Description = "Gelişmiş keşif ve gözetleme sistemleri.", ImageUrl = "/images/Patroller-IHA.jpg" },
            new ProductViewModel { Id = 2, Name = "Zırhlı Personel Taşıyıcı", Description = "Yüksek balistik koruma ve arazi kabiliyeti.", ImageUrl = "/images/GelkcEyWgAATVpF-aspect-ratio-1280-720.webp" },
            new ProductViewModel { Id = 3, Name = "Haberleşme Sistemleri", Description = "Kriptografik askeri haberleşme altyapısı.", ImageUrl = "/images/images.jpg" },
            new ProductViewModel { Id = 4, Name = "Güdümlü Füze Sistemi", Description = "Uzun menzilli hassas vuruş kabiliyeti.", ImageUrl = "/images/roketsan.webp" },
        };
        return View(products);
    }*/

    
/*public IActionResult ProductDetails(int id)
        {
            var product = new ProductViewModel 
            { 
                Id = id, 
                Name = "Taktik İHA X-1", 
                Description = "Gelişmiş keşif ve gözetleme sistemleri...", 
                ImageUrl = "/images/Patroller-IHA.jpg"
            };

            return View(product);
        }*/

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

    public IActionResult Index()
    {
        var products = new List<ProductViewModel>
        {
            new ProductViewModel { Id = 1, Name = "Taktik İHA X-1", Description = "Gelişmiş keşif ve gözetleme sistemleri.", ImageUrl = "/images/Patroller-IHA.jpg" },
            new ProductViewModel { Id = 2, Name = "Zırhlı Personel Taşıyıcı", Description = "Yüksek balistik koruma ve arazi kabiliyeti.", ImageUrl = "/images/GelkcEyWgAATVpF-aspect-ratio-1280-720.webp" },
            new ProductViewModel { Id = 3, Name = "Haberleşme Sistemleri", Description = "Kriptografik askeri haberleşme altyapısı.", ImageUrl = "/images/images.jpg" },
            new ProductViewModel { Id = 4, Name = "Güdümlü Füze Sistemi", Description = "Uzun menzilli hassas vuruş kabiliyeti.", ImageUrl = "/images/roketsan.webp" },
        };
        return View(products);
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