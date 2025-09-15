using KafkaMicroservices.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaMicroservices.InventoryService.Data;

/// <summary>
/// Entity Framework DbContext for Inventory Service
/// </summary>
public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure InventoryItem entity
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.ProductId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AvailableQuantity).IsRequired();
            entity.Property(e => e.ReservedQuantity).IsRequired();
        });

        // Seed some initial data
        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem { ProductId = "PROD001", ProductName = "Laptop", AvailableQuantity = 50, ReservedQuantity = 0 },
            new InventoryItem { ProductId = "PROD002", ProductName = "Mouse", AvailableQuantity = 100, ReservedQuantity = 0 },
            new InventoryItem { ProductId = "PROD003", ProductName = "Keyboard", AvailableQuantity = 75, ReservedQuantity = 0 },
            new InventoryItem { ProductId = "PROD004", ProductName = "Monitor", AvailableQuantity = 30, ReservedQuantity = 0 }
        );
    }
}
