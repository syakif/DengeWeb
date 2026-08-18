using DengeWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace DengeWeb.Data
{
    // Sınıfımız DbContext'ten miras alarak veritabanı yönetim yetenekleri kazanır
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Veritabanında oluşacak tablolarımızın tanımları
        public DbSet<Product> Products { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }
    }
}