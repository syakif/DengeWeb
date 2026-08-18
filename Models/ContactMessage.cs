using System;

namespace DengeWeb.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string Eposta { get; set; }
        public string Telefon { get; set; }
        public string Konu { get; set; }
        public string Mesaj { get; set; }
        
        // Mesajın panelden okunup okunmadığını takip etmek için
        public bool IsRead { get; set; } = false;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}