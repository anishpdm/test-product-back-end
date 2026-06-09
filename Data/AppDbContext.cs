using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Wireless Mouse", Description = "Ergonomic 2.4GHz mouse", Price = 799.00m, Stock = 50 },
            new Product { Id = 2, Name = "Mechanical Keyboard", Description = "RGB backlit, blue switches", Price = 2499.00m, Stock = 30 }
        );
    }
}
