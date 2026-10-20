using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using MediatR;

namespace eCommerce.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersHandler(
        IOrderQueries queries, 
        ICurrentUser user) 
        : IRequestHandler<GetAllOrdersQuery, IReadOnlyList<OrderResponse>>
    {
        private readonly IOrderQueries _queries = queries;
        private readonly ICurrentUser _user = user;

        public async Task<IReadOnlyList<OrderResponse>> Handle(
            GetAllOrdersQuery request, 
            CancellationToken ct)
        {
            ValidateCurrentUser();

            return await _queries.GetOrdersAsync(_user.UserId, _user.AnonymousId, ct);
        }
        private void ValidateCurrentUser()
        {
            if (string.IsNullOrWhiteSpace(_user.UserId) && _user.AnonymousId is null)
                throw new BusinessRuleException(
                    "A user or anonymous session is required.");

            if (!string.IsNullOrWhiteSpace(_user.UserId) && _user.AnonymousId is not null)
                throw new BusinessRuleException(
                    "An order cannot belong to both a user and an anonymous session.");
        }
    }
}
