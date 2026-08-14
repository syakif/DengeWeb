using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DengeWeb.Models;
using DengeWeb.Services;

using Microsoft.Extensions.Configuration;

namespace DengeWeb.Controllers;

public class HomeController : Controller
{
    private readonly IMailService _mailService;
    private readonly IConfiguration _config;

    public HomeController(IMailService mailService, IConfiguration config)
    {
        _mailService = mailService;
        _config = config;
    }

    //public IActionResult Index() => View();
    
    // About metodu artık dinamik veri yolluyor
    public IActionResult About()
    {
        var model = new AboutViewModel
        {
            HeaderTitle = _config["AboutPageContent:HeaderTitle"],
            HeaderSubtitle = _config["AboutPageContent:HeaderSubtitle"],
            MissionTitle = _config["AboutPageContent:MissionTitle"],
            MissionText = _config["AboutPageContent:MissionText"],
            MissionImage = _config["AboutPageContent:MissionImage"],
            EthicsTitle = _config["AboutPageContent:EthicsTitle"],
            EthicsText = _config["AboutPageContent:EthicsText"],
            EthicsImage = _config["AboutPageContent:EthicsImage"]
        };

        // Modeli sayfaya (View) gönderiyoruz
        return View(model); 
    }

    public IActionResult Products()
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
    public async Task<IActionResult> Contact(ContactViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _mailService.SendEmailAsync(model);
                TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi. En kısa sürede dönüş yapılacaktır.";
                return RedirectToAction("Contact");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Mail gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
            }
        }
        return View(model);
    }
}