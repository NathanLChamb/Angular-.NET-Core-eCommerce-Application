using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Features.Orders.Filters;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Shared;
using eCommerce.Domain.Order;
using eCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Queries
{
    public class OrderQueries(
        eCommerceContext context) 
        : IOrderQueries
    {
        private readonly eCommerceContext _context = context;

        public async Task<OrderResponse?> GetByIdAsync(
            int id, 
            string? userId, 
            Guid? anonymousId, 
            CancellationToken ct)
        {
            ValidateIdentity(userId, anonymousId);

            var query = _context.Orders
                .AsNoTracking()
                .Where(o => o.Id == id);

            query = !string.IsNullOrWhiteSpace(userId)
                ? query.Where(o => o.UserId == userId)
                : query.Where(o => o.AnonymousId == anonymousId);

            return await query
                .Select(o => new OrderResponse
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber.Value,
                    ShippingAddress = o.ShippingAddress.Value,
                    Status = o.Status.ToString(),
                    OrderDate = o.OrderDate,
                    TotalPrice = o.Items.Sum(
                        i => i.UnitPrice.Amount * i.Quantity),

                    Items = o.Items.Select(i => new OrderItemResponse
                    {
                        Id = i.Id,
                        ProductVariantId = i.ProductVariantId,
                        ProductName = i.Product.ProductName,
                        Sku = i.Product.Sku,
                        ImageUrl = i.Product.Image == null
                            ? null
                            : i.Product.Image.ToString(),
                        UnitPrice = i.UnitPrice.Amount,
                        Quantity = i.Quantity,
                        LineTotal = i.UnitPrice.Amount * i.Quantity
                    }).ToList()
                })
                .SingleOrDefaultAsync(ct);
        }

        public async Task<IReadOnlyList<OrderResponse>> GetOrdersAsync(
            string? userId,
            Guid? anonymousId,
            CancellationToken ct)
        {
            ValidateIdentity(userId, anonymousId);

            var query = _context.Orders.AsNoTracking();

            query = !string.IsNullOrWhiteSpace(userId)
                ? query.Where(o => o.UserId == userId)
                : query.Where(o => o.AnonymousId == anonymousId);

            return await query
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderResponse
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber.Value,
                    ShippingAddress = o.ShippingAddress.Value,
                    Status = o.Status.ToString(),
                    OrderDate = o.OrderDate,
                    TotalPrice = o.Items.Sum(
                        i => i.UnitPrice.Amount * i.Quantity),

                    Items = o.Items.Select(i => new OrderItemResponse
                    {
                        Id = i.Id,
                        ProductVariantId = i.ProductVariantId,
                        ProductName = i.Product.ProductName,
                        Sku = i.Product.Sku,
                        ImageUrl = i.Product.Image == null
                            ? null
                            : i.Product.Image.ToString(),
                        UnitPrice = i.UnitPrice.Amount,
                        Quantity = i.Quantity,
                        LineTotal = i.UnitPrice.Amount * i.Quantity
                    }).ToList()
                })
                .ToListAsync(ct);
        }

        public async Task<AdminOrderResponse?> AdminGetByIdAsync(
            int id, 
            CancellationToken ct)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new AdminOrderResponse
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber.Value,
                    ShippingAddress = o.ShippingAddress.Value,
                    Status = o.Status.ToString(),
                    OrderDate = o.OrderDate,
                    TotalPrice = o.Items.Sum(
                        i => i.UnitPrice.Amount * i.Quantity),

                    Items = o.Items.Select(i => new OrderItemResponse
                    {
                        Id = i.Id,
                        ProductVariantId = i.ProductVariantId,
                        ProductName = i.Product.ProductName,
                        Sku = i.Product.Sku,
                        ImageUrl = i.Product.Image == null
                            ? null
                            : i.Product.Image.ToString(),
                        UnitPrice = i.UnitPrice.Amount,
                        Quantity = i.Quantity,
                        LineTotal = i.UnitPrice.Amount * i.Quantity
                    }).ToList()
                })
                .SingleOrDefaultAsync(ct)
                ?? throw new NotFoundException(
                    "Order not found.");

            return order;
        }

        public async Task<PagedResult<AdminOrderResponse>> AdminGetOrdersAsync(
            string? userId,
            Guid? anonymousId,
            OrderSearchFilter filter,
            CancellationToken ct)
        {
            ValidateIdentity(userId, anonymousId);

            if (filter.PageNumber < 1 || filter.PageSize < 1)
                throw new BusinessRuleException(
                    "Page number and page size must be greater than zero.");

            var query = _context.Orders.AsNoTracking();

            query = !string.IsNullOrWhiteSpace(userId)
                ? query.Where(o => o.UserId == userId)
                : query.Where(o => o.AnonymousId == anonymousId);

            query = filter.Status switch
            {
                OrderSort.Working => query.Where(o =>
                    o.Status == OrderStatus.Pending ||
                    o.Status == OrderStatus.Processing ||
                    o.Status == OrderStatus.Shipped),

                OrderSort.Completed => query.Where(o =>
                    o.Status == OrderStatus.Delivered),

                OrderSort.Cancelled => query.Where(o =>
                    o.Status == OrderStatus.Cancelled),

                _ => query
            };

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(o => new AdminOrderResponse
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber.Value,
                    ShippingAddress = o.ShippingAddress.Value,
                    Status = o.Status.ToString(),
                    OrderDate = o.OrderDate,
                    TotalPrice = o.Items.Sum(
                        i => i.UnitPrice.Amount * i.Quantity),

                    Items = o.Items.Select(i => new OrderItemResponse
                    {
                        Id = i.Id,
                        ProductVariantId = i.ProductVariantId,
                        ProductName = i.Product.ProductName,
                        Sku = i.Product.Sku,
                        ImageUrl = i.Product.Image == null
                            ? null
                            : i.Product.Image.ToString(),
                        UnitPrice = i.UnitPrice.Amount,
                        Quantity = i.Quantity,
                        LineTotal = i.UnitPrice.Amount * i.Quantity
                    }).ToList()
                })
                .ToListAsync(ct);

            return new PagedResult<AdminOrderResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        private static void ValidateIdentity(
            string? userId,
            Guid? anonymousId)
        {
            if (string.IsNullOrWhiteSpace(userId) && anonymousId is null)
                throw new ArgumentException(
                    "Either userId or anonymousId must be provided.");

            if (!string.IsNullOrWhiteSpace(userId) && anonymousId is not null)
                throw new ArgumentException(
                    "An order cannot be identified by both userId and anonymousId.");
        }
    }
}
