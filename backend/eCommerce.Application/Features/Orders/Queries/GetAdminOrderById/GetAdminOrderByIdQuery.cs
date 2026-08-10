using MediatR;

namespace eCommerce.Application.Features.Orders.Queries.GetAdminOrderById
{
    public record GetAdminOrderByIdQuery(
        int Id
    ) : IRequest<ReadOrderFromAdminDto>;
}
