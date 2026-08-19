using DengeWeb.Data;
using DengeWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DengeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SiteSettingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiteSettingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kayıtlı Dilleri Listele
        public IActionResult Index()
        {
            var settings = _context.SiteSettings.ToList();
            return View(settings);
        }

        public IActionResult Create()
        {
            return View(new SiteSetting());
        }

        [HttpPost]
        public IActionResult Create(SiteSetting model)
        {
            _context.SiteSettings.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var setting = _context.SiteSettings.Find(id);
            if (setting == null) return NotFound();
            return View(setting);
        }

        [HttpPost]
        public IActionResult Edit(SiteSetting model)
        {
            _context.SiteSettings.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}