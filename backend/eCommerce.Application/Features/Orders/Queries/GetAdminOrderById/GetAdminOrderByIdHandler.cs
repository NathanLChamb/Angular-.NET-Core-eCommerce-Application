using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Orders.Mappings;
using eCommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Features.Orders.Queries.GetAdminOrderById
{
    public class GetAdminOrderByIdHandler : IRequestHandler<GetAdminOrderByIdQuery, ReadOrderFromAdminDto>
    {
        private readonly IeCommerceContext _context;

        public GetAdminOrderByIdHandler(IeCommerceContext context)
        {
            _context = context;
        }

        public async Task<ReadOrderFromAdminDto> Handle(GetAdminOrderByIdQuery request, CancellationToken ct)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Id == request.Id)
                .ToAdminOrderDto()
                .SingleOrDefaultAsync(ct);
            if (order == null) throw new NotFoundException("Order not found.");

            return order;
        }
    }
}
