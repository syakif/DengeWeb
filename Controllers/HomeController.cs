using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DengeWeb.Models;
using DengeWeb.Services;

namespace DengeWeb.Controllers;

public class HomeController : Controller
{
    private readonly IMailService _mailService;

    public HomeController(IMailService mailService)
    {
        _mailService = mailService;
    }

    public IActionResult Index() => View();
    
    public IActionResult About() => View();

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