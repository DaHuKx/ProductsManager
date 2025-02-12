using Microsoft.EntityFrameworkCore;
using ProductsManager.Domain.DbEntities;
using ProductsManager.Infrastructure.DataBase.Entities;

namespace ProductsManager.Infrastructure.DataBase
{
    public class ProductsManagerDb : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<BotUser> BotUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                        .HasMany(p => p.Trades)
                        .WithOne(p => p.Product)
                        .HasForeignKey(p => p.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=ProjectManager;User Id=postgres;Password=w3s4m1p3;");
        }
    }
}
