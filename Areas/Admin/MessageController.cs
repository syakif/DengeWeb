using DengeWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DengeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // DİKKAT: Giriş yapmayan kimse bu sayfaları GÖREMEZ!
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tüm mesajları listele
        public IActionResult Index()
        {
            // En yeni mesaj en üstte gelsin
            var messages = _context.ContactMessages.OrderByDescending(m => m.CreatedDate).ToList();
            return View(messages);
        }

        // Mesajı Oku
        public IActionResult Details(int id)
        {
            var message = _context.ContactMessages.FirstOrDefault(m => m.Id == id);
            if (message == null) return NotFound();

            // Mesaja tıklandığında okunmadıysa "Okundu" olarak işaretle
            if (!message.IsRead)
            {
                message.IsRead = true;
                _context.SaveChanges();
            }

            return View(message);
        }

        // Mesajı Sil
        public IActionResult Delete(int id)
        {
            var message = _context.ContactMessages.FirstOrDefault(m => m.Id == id);
            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}