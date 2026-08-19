using System.Collections.Generic;
using DengeWeb.Models;

namespace DengeWeb.ViewModels
{
    public class HomeIndexViewModel
    {
        // 1. Verimiz: Site Ayarları
        public SiteSetting SiteSettings { get; set; }
        
        // 2. Verimiz: Ürünler Listesi
        public List<Product> Products { get; set; }
    }
}