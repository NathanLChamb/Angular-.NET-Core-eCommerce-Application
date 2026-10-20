using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Interfaces.Queries;
using MediatR;

namespace eCommerce.Application.Features.Orders.Queries.GetAdminOrderById
{
    public class GetAdminOrderByIdHandler(
        IOrderQueries queries)
        : IRequestHandler<GetAdminOrderByIdQuery, AdminOrderResponse?>
    {
        private readonly IOrderQueries _queries = queries;

        public async Task<AdminOrderResponse?> Handle(
            GetAdminOrderByIdQuery request, 
            CancellationToken ct)
        {
            var order = await _queries.AdminGetByIdAsync(request.Id, ct);

            return order;
        }
    }
}
