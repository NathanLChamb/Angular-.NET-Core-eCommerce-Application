using eCommerce.Application.Features.Orders.Filters;
using eCommerce.Application.Features.Orders.Mappings;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Shared;
using eCommerce.Domain.Order;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, PagedResult<ReadOrderFromAdminDto>>
    {
        private readonly IeCommerceContext _context;
        public GetAllOrdersHandler(IeCommerceContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<ReadOrderFromAdminDto>> Handle(GetAllOrdersQuery request, CancellationToken ct)
        {
            var ordersQuery = _context.Orders
                .AsNoTracking()
                .AsQueryable();

            switch (request.Filter.Status)
            {
                case OrderSort.Working:
                    ordersQuery = ordersQuery.Where(o =>
                        o.Status == OrderStatus.Pending ||
                        o.Status == OrderStatus.Processing ||
                        o.Status == OrderStatus.Shipped);
                    break;

                case OrderSort.Completed:
                    ordersQuery = ordersQuery.Where(o =>
                        o.Status == OrderStatus.Delivered);
                    break;

                case OrderSort.Cancelled:
                    ordersQuery = ordersQuery.Where(o =>
                        o.Status == OrderStatus.Cancelled);
                    break;

                case OrderSort.All: default:
                    break;
            }

            var totalCount = await ordersQuery
                .CountAsync(ct);

            var orders = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .Skip((request.Filter.PageNumber - 1) * request.Filter.PageSize)
                .Take(request.Filter.PageSize)
                .ToAdminOrderDto()
                .ToListAsync(ct);

            return new PagedResult<ReadOrderFromAdminDto>
            {
                Items = orders,
                TotalCount = totalCount,
                PageNumber = request.Filter.PageNumber,
                PageSize = request.Filter.PageSize
            };
        }
    }
}
