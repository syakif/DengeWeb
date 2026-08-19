using DengeWeb.Data;
using DengeWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; // IWebHostEnvironment için eklendi
using Microsoft.AspNetCore.Http; // IFormFile (Dosya işlemleri) için eklendi
using Microsoft.AspNetCore.Mvc;
using System.IO; // Path (Klasör yolları) için eklendi

namespace DengeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env; // Sunucudaki (wwwroot) klasör yolunu bulmak için

        public ProductController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var products = _context.Products.OrderByDescending(p => p.CreatedDate).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View(new Product());
        }

        // DİKKAT: Artık dosyaları yakalamak için IFormFile parametreleri ekledik
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile? imageUpload, IFormFile? brochureUpload, List<IFormFile>? galleryUploads)
        {
            // 1. ANA GÖRSELİ YÜKLE
            if (imageUpload != null && imageUpload.Length > 0)
            {
                // Rastgele benzersiz bir isim veriyoruz (Örn: 5a4s6d4-araba.jpg) çakışma olmasın diye
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageUpload.FileName;
                
                // wwwroot/images klasörünün yolunu bul
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder); // Klasör yoksa oluştur
                
                // Dosyayı sunucuya (wwwroot/images) fiziksel olarak kopyala
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUpload.CopyToAsync(fileStream);
                }
                
                // Veritabanına kaydedilecek olan string URL'i belirliyoruz
                product.ImageUrl = "/images/" + uniqueFileName;
            }

            // 2. BROŞÜRÜ YÜKLE
            if (brochureUpload != null && brochureUpload.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + brochureUpload.FileName;
                string docsFolder = Path.Combine(_env.WebRootPath, "docs");
                if (!Directory.Exists(docsFolder)) Directory.CreateDirectory(docsFolder);
                
                string filePath = Path.Combine(docsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await brochureUpload.CopyToAsync(fileStream);
                }
                product.BrochureUrl = "/docs/" + uniqueFileName;
            }

            // 3. ÇOKLU GALERİ RESİMLERİNİ YÜKLE
            if (galleryUploads != null && galleryUploads.Count > 0)
            {
                product.GalleryUrls = new List<string>();
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");

                foreach (var file in galleryUploads)
                {
                    if (file.Length > 0)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        // Her yüklenen resmin yolunu listeye ekle
                        product.GalleryUrls.Add("/images/" + uniqueFileName);
                    }
                }
            }

            // Veritabanına kaydet
            product.CreatedDate = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // DÜZENLEME SAYFASINI AÇMA (Hiçbir değişiklik yok, sadece ürünü yolluyoruz)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // DÜZENLEMEYİ KAYDETME (Asıl sihir burada başlıyor)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageUpload, IFormFile? brochureUpload, List<IFormFile>? galleryUploads)
        {
            if (id != product.Id) return NotFound();

            // 1. Önce veritabanındaki orijinal (eski) ürünü buluyoruz
            var existingProduct = _context.Products.Find(id);
            if (existingProduct == null) return NotFound();

            // 2. Sadece metinsel değerleri güncelliyoruz
            existingProduct.Name = product.Name;
            existingProduct.ShortDescription = product.ShortDescription;
            existingProduct.LongDescription = product.LongDescription;
            existingProduct.IsActive = product.IsActive;

            // 3. ANA GÖRSEL GÜNCELLEMESİ (Eğer yeni dosya seçilmişse)
            if (imageUpload != null && imageUpload.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageUpload.FileName;
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUpload.CopyToAsync(fileStream);
                }
                
                // Sadece yeni resim yüklendiyse adresi değiştir, yoksa eski resim aynı kalır.
                existingProduct.ImageUrl = "/images/" + uniqueFileName;
            }

            // 4. BROŞÜR GÜNCELLEMESİ (Eğer yeni dosya seçilmişse)
            if (brochureUpload != null && brochureUpload.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + brochureUpload.FileName;
                string docsFolder = Path.Combine(_env.WebRootPath, "docs");
                if (!Directory.Exists(docsFolder)) Directory.CreateDirectory(docsFolder);
                
                string filePath = Path.Combine(docsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await brochureUpload.CopyToAsync(fileStream);
                }
                existingProduct.BrochureUrl = "/docs/" + uniqueFileName;
            }

            // 5. GALERİ GÜNCELLEMESİ (Yeni seçilenleri eskilerin yanına "ekleriz")
            if (galleryUploads != null && galleryUploads.Count > 0)
            {
                // Eğer daha önce hiç galeri resmi yoksa null patlaması yememek için listeyi başlat
                if (existingProduct.GalleryUrls == null) 
                {
                    existingProduct.GalleryUrls = new List<string>();
                }

                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");

                foreach (var file in galleryUploads)
                {
                    if (file.Length > 0)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        // Yeni resmi listenin sonuna ekliyoruz
                        existingProduct.GalleryUrls.Add("/images/" + uniqueFileName);
                    }
                }
            }

            // Değişiklikleri SQL'e gönder ve işlemi bitir
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}