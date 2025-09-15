using KafkaMicroservices.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaMicroservices.OrderService.Data;

/// <summary>
/// Entity Framework DbContext for Order Service
/// </summary>
public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            // Configure OrderItems as owned entities (JSON column)
            entity.OwnsMany(e => e.Items, items =>
            {
                items.Property(i => i.ProductId).IsRequired().HasMaxLength(50);
                items.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
                items.Property(i => i.Quantity).IsRequired();
                items.Property(i => i.Price).HasColumnType("decimal(18,2)").IsRequired();
            });
        });
    }
}
