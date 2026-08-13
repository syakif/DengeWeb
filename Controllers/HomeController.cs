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

    public IActionResult Index() => View();
    
    // About metodu artık dinamik veri yolluyor
    public IActionResult About()
    {
        var model = new AboutViewModel
        {
            HeaderTitle = _config["AboutPageContent:HeaderTitle"],
            HeaderSubtitle = _config["AboutPageContent:HeaderSubtitle"],
            MissionTitle = _config["AboutPageContent:MissionTitle"],
            MissionText = _config["AboutPageContent:MissionText"],
            MissionImage = _config["AboutPageContent:MissionImage"]
        };

        // Modeli sayfaya (View) gönderiyoruz
        return View(model); 
    }

    public IActionResult Products()
    {
        var products = new List<ProductViewModel>
        {
            new ProductViewModel { Id = 1, Name = "Taktik İHA X-1", Description = "Gelişmiş keşif ve gözetleme sistemleri.", ImageUrl = "https://via.placeholder.com/300x200?text=IHA" },
            new ProductViewModel { Id = 2, Name = "Zırhlı Personel Taşıyıcı", Description = "Yüksek balistik koruma ve arazi kabiliyeti.", ImageUrl = "https://via.placeholder.com/300x200?text=ZPT" },
            new ProductViewModel { Id = 3, Name = "Haberleşme Sistemleri", Description = "Kriptolu askeri haberleşme altyapısı.", ImageUrl = "https://via.placeholder.com/300x200?text=Radar" }
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