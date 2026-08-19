using DengeWeb.Data;
using DengeWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DengeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.OrderByDescending(p => p.CreatedDate).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product, string galleryUrlsString)
        {
            // Virgülle ayrılmış string'i List<string>'e çeviriyoruz
            if (!string.IsNullOrWhiteSpace(galleryUrlsString))
            {
                product.GalleryUrls = galleryUrlsString.Split(',').Select(x => x.Trim()).ToList();
            }

            product.CreatedDate = DateTime.Now;
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            // List<string>'i tekrar virgüllü string'e çevirip view'a yolluyoruz
            ViewBag.GalleryUrlsString = product.GalleryUrls != null ? string.Join(", ", product.GalleryUrls) : "";
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product, string galleryUrlsString)
        {
            if (!string.IsNullOrWhiteSpace(galleryUrlsString))
            {
                product.GalleryUrls = galleryUrlsString.Split(',').Select(x => x.Trim()).ToList();
            }
            else
            {
                product.GalleryUrls = new List<string>();
            }

            _context.Products.Update(product);
            _context.SaveChanges();
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