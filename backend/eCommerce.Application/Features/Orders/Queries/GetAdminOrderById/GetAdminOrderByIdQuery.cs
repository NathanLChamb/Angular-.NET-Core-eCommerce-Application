using eCommerce.Application.Features.Orders.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Orders.Queries.GetAdminOrderById
{
    public record GetAdminOrderByIdQuery(
        int Id
    ) : IRequest<AdminOrderResponse?>;
}
