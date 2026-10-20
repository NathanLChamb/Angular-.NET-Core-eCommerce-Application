using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Features.Orders.Filters;
using eCommerce.Application.Shared;

namespace eCommerce.Application.Interfaces.Queries
{
    public interface IOrderQueries
    {
        Task<OrderResponse?> GetByIdAsync(
           int id,
           string? userId,
           Guid? anonymousId,
           CancellationToken ct);

        Task<IReadOnlyList<OrderResponse>>GetOrdersAsync(
            string? userId,
            Guid? anonymousId,
            CancellationToken ct);

        Task<AdminOrderResponse?> AdminGetByIdAsync(
            int Id,
            CancellationToken ct);

        Task<PagedResult<AdminOrderResponse>> AdminGetOrdersAsync(
            string? userId,
            Guid? anonymousId,
            OrderSearchFilter filter,
            CancellationToken ct);
    }
}
