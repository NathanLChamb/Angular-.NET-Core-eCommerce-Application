using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Domain.Order;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, ReadOrderDto>
    {
        private readonly IeCommerceContext _context;
        private readonly ITransactionManager _transaction;

        public CreateOrderHandler(IeCommerceContext context, ITransactionManager transaction)
        {
            _context = context;
            _transaction = transaction;
        }


        public async Task<ReadOrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
        {
            await _transaction.BeginTransactionAsync(ct);

            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                        .ThenInclude(ci => ci.ProductVariant)
                            .ThenInclude(pv => pv.Product)
                    .FirstOrDefaultAsync(
                        c => c.UserId == request.UserId,
                        ct);

                if (cart == null) throw new NotFoundException("Cart not found.");
                if (!cart.Items.Any()) throw new BusinessRuleException("Cannot checkout an empty cart.");

                foreach (var item in cart.Items)
                {
                    if (item.Quantity > item.ProductVariant.StockQuantity)
                        throw new BusinessRuleException($"Insufficient stock for {item.ProductVariant.Sku}.");
                }

                var order = new Order
                {
                    UserId = request.UserId,
                    OrderNumber = GenerateOrderNumber(),
                    ShippingAddress = request.ShippingAddress,
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow
                };

                foreach (var item in cart.Items)
                {
                    var variant = item.ProductVariant;

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductVariantId = variant.Id,
                        ProductName = variant.Product.Name,
                        Sku = variant.Sku,
                        PriceAtPurchase = variant.Price,
                        Quantity = item.Quantity
                    });

                    variant.StockQuantity -= item.Quantity;
                }

                order.TotalPrice = order.OrderItems.Sum(i => i.PriceAtPurchase * i.Quantity);

                _context.Orders.Add(order);

                cart.Items.Clear();
                cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(ct);
                await _transaction.CommitTransactionAsync(ct);

                return new ReadOrderDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    ShippingAddress = order.ShippingAddress,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status.ToString(),
                    OrderDate = order.OrderDate,
                    Items = order.OrderItems.Select(i => new ReadOrderItemDto
                    {
                        Id = i.Id,
                        ProductName = i.ProductName,
                        Sku = i.Sku,
                        PriceAtPurchase = i.PriceAtPurchase,
                        Quantity = i.Quantity,
                        TotalPrice = i.PriceAtPurchase * i.Quantity
                    }).ToList()
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                await _transaction.RollbackTransactionAsync(ct);
                throw new ConflictException("The stock changed while processing your order. Please try again.");
            }
            catch
            {
                await _transaction.RollbackTransactionAsync(ct);
                throw;
            }
        }
        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid()
                .ToString()[..8]
                .ToUpper()}";
        }
    }
}
