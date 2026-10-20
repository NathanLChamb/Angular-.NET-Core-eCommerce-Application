using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Order;
using MediatR;

namespace eCommerce.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusHandler(
    IOrderRepository orders,
    IOrderQueries queries,
    ICurrentUser user,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOrderStatusCommand, OrderResponse>
    {
        private readonly IOrderRepository _orders = orders;
        private readonly IOrderQueries _queries = queries;
        private readonly ICurrentUser _user = user;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<OrderResponse> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
        {
            var order = await _orders.GetByIdAsync(
                request.OrderId,
                _user.UserId,
                _user.AnonymousId,
                ct)
                ?? throw new NotFoundException(
                    "Order not found.");

            try
            {
                switch (request.Status)
                {
                    case OrderStatus.Processing:
                        order.Process();
                        break;

                    case OrderStatus.Shipped:
                        order.Ship();
                        break;

                    case OrderStatus.Delivered:
                        order.Deliver();
                        break;

                    case OrderStatus.Cancelled:
                        order.Cancel();
                        break;

                    default:
                        throw new BusinessRuleException(
                            $"Transition to status '{request.Status}' is not supported.");
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessRuleException(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return await _queries.GetByIdAsync(
                order.Id,
                _user.UserId,
                _user.AnonymousId,
                ct)
                ?? throw new NotFoundException(
                    "The order could not be found.");
        }
    }
}