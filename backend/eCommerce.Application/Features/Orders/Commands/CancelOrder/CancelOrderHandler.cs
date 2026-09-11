using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Orders.Commands.CancelOrder;
using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Features.Orders.Mappings;
using eCommerce.Application.Interfaces;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using eCommerce.Domain.Order;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, ReadOrderDto>
{
    private readonly IeCommerceContext _context;
    public CancelOrderHandler(IeCommerceContext context)
    {
        _context = context;
    }

    public async Task<ReadOrderDto> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, ct);

        if (order == null) throw new NotFoundException("Order not found.");

        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
            throw new BusinessRuleException("Only pending or processing orders can be cancelled.");
        
        foreach (var item in order.OrderItems)
        {
            var variant = await _context.ProductVariants
                .FirstAsync(pv => pv.Id == item.ProductVariantId, ct);
            variant.IncreaseStock(new StockQuantity(item.Quantity));
        }
        order.Status = OrderStatus.Cancelled;

        await _context.SaveChangesAsync(ct);

        return await _context.Orders
            .Where(o => o.Id == order.Id)
            .ToOrderDto()
            .FirstAsync(ct);
    }
}
