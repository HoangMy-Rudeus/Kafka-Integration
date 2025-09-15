using KafkaMicroservices.Shared.Application.Interfaces;
using KafkaMicroservices.Shared.Domain.Entities;
using KafkaMicroservices.Shared.Domain.ValueObjects;
using KafkaMicroservices.Shared.Infrastructure.Interfaces;

namespace KafkaMicroservices.OrderService.Application.Services;

/// <summary>
/// Application service for order management following clean architecture principles
/// </summary>
public class OrderApplicationService : IOrderApplicationService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventPublisher _domainEventPublisher;
    private readonly IAppLogger<OrderApplicationService> _logger;

    public OrderApplicationService(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IDomainEventPublisher domainEventPublisher,
        IAppLogger<OrderApplicationService> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Order> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating order for customer {CustomerId} with {ItemCount} items", 
            command.CustomerId, command.Items.Count());

        try
        {
            // Create order items from command
            var orderItems = command.Items.Select(item => 
                new OrderItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice)).ToList();

            // Create order entity
            var order = new Order(command.CustomerId, orderItems);

            // Add to repository
            await _orderRepository.AddAsync(order, cancellationToken);

            // Publish domain events before saving
            foreach (var domainEvent in order.DomainEvents)
            {
                await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Clear domain events after successful save
            order.ClearDomainEvents();

            _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerId}", 
                order.Id, command.CustomerId);

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order for customer {CustomerId}", command.CustomerId);
            throw;
        }
    }

    public async Task<Order?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving order {OrderId}", orderId);
        return await _orderRepository.GetByIdAsync(orderId, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetCustomerOrdersAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving orders for customer {CustomerId}", customerId);
        return await _orderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
    }

    public async Task ConfirmOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Confirming order {OrderId}", orderId);

        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                throw new InvalidOperationException($"Order {orderId} not found");
            }

            order.ConfirmOrder();

            // Publish domain events
            foreach (var domainEvent in order.DomainEvents)
            {
                await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
            }

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            order.ClearDomainEvents();

            _logger.LogInformation("Order {OrderId} confirmed successfully", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to confirm order {OrderId}", orderId);
            throw;
        }
    }

    public async Task CompleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Completing order {OrderId}", orderId);

        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                throw new InvalidOperationException($"Order {orderId} not found");
            }

            order.CompleteOrder();

            // Publish domain events
            foreach (var domainEvent in order.DomainEvents)
            {
                await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
            }

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            order.ClearDomainEvents();

            _logger.LogInformation("Order {OrderId} completed successfully", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to complete order {OrderId}", orderId);
            throw;
        }
    }

    public async Task CancelOrderAsync(Guid orderId, string reason, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling order {OrderId} with reason: {Reason}", orderId, reason);

        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                throw new InvalidOperationException($"Order {orderId} not found");
            }

            order.CancelOrder(reason);

            // Publish domain events
            foreach (var domainEvent in order.DomainEvents)
            {
                await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
            }

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            order.ClearDomainEvents();

            _logger.LogInformation("Order {OrderId} cancelled successfully", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel order {OrderId}", orderId);
            throw;
        }
    }
}