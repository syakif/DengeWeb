using DengeWeb.Data;
using DengeWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; // IWebHostEnvironment için eklendi
using Microsoft.AspNetCore.Http; // IFormFile için eklendi
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DengeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SiteSettingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        // IWebHostEnvironment, wwwroot klasörünün yolunu bulmamızı sağlar
        public SiteSettingController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // --- YARDIMCI METOT: Dosyayı sunucuya yükler ve URL'sini döndürür ---
        private async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            // wwwroot/uploads klasörünün yolunu belirliyoruz
            string uploadDir = Path.Combine(_env.WebRootPath, "uploads");
            
            // Klasör yoksa oluştur
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Dosya isimleri çakışmasın diye benzersiz bir isim (Guid) üretiyoruz
            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadDir, fileName);

            // Dosyayı fiziksel olarak sunucuya kopyalıyoruz
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Veritabanına kaydedilecek olan URL yolunu döndürüyoruz
            return "/uploads/" + fileName;
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
        public async Task<IActionResult> Create(SiteSetting model, 
            IFormFile faviconUpload, 
            IFormFile headerLogoUpload, 
            IFormFile headerWhiteLogoUpload, 
            IFormFile footerLogoUpload, 
            IFormFile indexStockVideoUpload, 
            IFormFile indexAboutImageUpload, 
            IFormFile aboutMissionVisionImageUpload, 
            IFormFile aboutValuesImageUpload)
        {
            // Yeni dosyalar yüklendiyse URL'lerini modele atıyoruz
            if (faviconUpload != null) model.FaviconUrl = await UploadFileAsync(faviconUpload);
            if (headerLogoUpload != null) model.HeaderLogoUrl = await UploadFileAsync(headerLogoUpload);
            if (headerWhiteLogoUpload != null) model.HeaderWhiteLogoUrl = await UploadFileAsync(headerWhiteLogoUpload);
            if (footerLogoUpload != null) model.FooterLogoUrl = await UploadFileAsync(footerLogoUpload);
            if (indexStockVideoUpload != null) model.IndexStockVideoUrl = await UploadFileAsync(indexStockVideoUpload);
            if (indexAboutImageUpload != null) model.IndexAboutImageUrl = await UploadFileAsync(indexAboutImageUpload);
            if (aboutMissionVisionImageUpload != null) model.AboutMissionVisionImageUrl = await UploadFileAsync(aboutMissionVisionImageUpload);
            if (aboutValuesImageUpload != null) model.AboutValuesImageUrl = await UploadFileAsync(aboutValuesImageUpload);

            _context.SiteSettings.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var setting = _context.SiteSettings.Find(id);
            if (setting == null) return NotFound();
            return View(setting);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SiteSetting model, 
            IFormFile faviconUpload, 
            IFormFile headerLogoUpload, 
            IFormFile headerWhiteLogoUpload, 
            IFormFile footerLogoUpload, 
            IFormFile indexStockVideoUpload, 
            IFormFile indexAboutImageUpload, 
            IFormFile aboutMissionVisionImageUpload, 
            IFormFile aboutValuesImageUpload)
        {
            // Formdan (hidden input ile) eski URL'ler zaten 'model' içinde geliyor.
            // Sadece YENİ bir dosya yüklenmişse mevcut URL'nin üzerine yazıyoruz.
            if (faviconUpload != null) model.FaviconUrl = await UploadFileAsync(faviconUpload);
            if (headerLogoUpload != null) model.HeaderLogoUrl = await UploadFileAsync(headerLogoUpload);
            if (headerWhiteLogoUpload != null) model.HeaderWhiteLogoUrl = await UploadFileAsync(headerWhiteLogoUpload);
            if (footerLogoUpload != null) model.FooterLogoUrl = await UploadFileAsync(footerLogoUpload);
            if (indexStockVideoUpload != null) model.IndexStockVideoUrl = await UploadFileAsync(indexStockVideoUpload);
            if (indexAboutImageUpload != null) model.IndexAboutImageUrl = await UploadFileAsync(indexAboutImageUpload);
            if (aboutMissionVisionImageUpload != null) model.AboutMissionVisionImageUrl = await UploadFileAsync(aboutMissionVisionImageUpload);
            if (aboutValuesImageUpload != null) model.AboutValuesImageUrl = await UploadFileAsync(aboutValuesImageUpload);

            _context.SiteSettings.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}