using System;

namespace DengeWeb.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string? ImageUrl { get; set; }
        public string? BrochureUrl { get; set; }
        public List<string> GalleryUrls { get; set; } = new List<string>();
        
        // Ürünü panelden gizleyip/açabilmek için
        public bool IsActive { get; set; } = true; 
        
        // Ürünün sisteme eklenme tarihi
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}