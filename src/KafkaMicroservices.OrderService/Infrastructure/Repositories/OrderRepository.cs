using KafkaMicroservices.OrderService.Data;
using KafkaMicroservices.OrderService.Infrastructure.Mapping;
using KafkaMicroservices.Shared.Application.Interfaces;
using KafkaMicroservices.Shared.Domain.Entities;
using KafkaMicroservices.Shared.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using DomainOrder = KafkaMicroservices.Shared.Domain.Entities.Order;
using DbOrder = KafkaMicroservices.Shared.Models.Order;

namespace KafkaMicroservices.OrderService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Order entity using Entity Framework
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<DomainOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbOrder = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        
        return dbOrder != null ? OrderMapper.ToDomainEntity(dbOrder) : null;
    }

    public async Task<IEnumerable<DomainOrder>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        var dbOrders = await _context.Orders
            .Where(o => o.CustomerId == customerId.Value)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
        
        return dbOrders.Select(OrderMapper.ToDomainEntity);
    }

    public async Task<IEnumerable<DomainOrder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dbOrders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
        
        return dbOrders.Select(OrderMapper.ToDomainEntity);
    }

    public async Task<DomainOrder> AddAsync(DomainOrder order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var dbOrder = OrderMapper.ToDbModel(order);
        var entry = await _context.Orders.AddAsync(dbOrder, cancellationToken);
        return OrderMapper.ToDomainEntity(entry.Entity);
    }

    public Task UpdateAsync(DomainOrder order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        // Find the existing entity in the context
        var existingDbOrder = _context.Orders.Local.FirstOrDefault(o => o.Id == order.Id);
        if (existingDbOrder != null)
        {
            // Update the existing entity
            OrderMapper.UpdateDbModel(existingDbOrder, order);
        }
        else
        {
            // Convert to db model and update
            var dbOrder = OrderMapper.ToDbModel(order);
            _context.Orders.Update(dbOrder);
        }
        
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(DomainOrder order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        // Find the existing entity to delete
        var existingDbOrder = await _context.Orders.FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);
        if (existingDbOrder != null)
        {
            _context.Orders.Remove(existingDbOrder);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AnyAsync(o => o.Id == id, cancellationToken);
    }
}