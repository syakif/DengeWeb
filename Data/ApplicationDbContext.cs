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
        public DbSet<AdminUser> AdminUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
                entity.HasIndex(x => x.Email).IsUnique();
            });
        }
    }
}