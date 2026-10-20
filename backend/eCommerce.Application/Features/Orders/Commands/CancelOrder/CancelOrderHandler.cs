using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using MediatR;

namespace eCommerce.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderHandler(
    IOrderRepository orders,
    IOrderQueries queries,
    IProductRepository products,
    ICurrentUser user,
    IUnitOfWork unitOfWork,
    ITransactionManager transaction)
    : IRequestHandler<CancelOrderCommand, OrderResponse>
    {
        private readonly IOrderRepository _orders = orders;
        private readonly IOrderQueries _queries = queries;
        private readonly IProductRepository _products = products;
        private readonly ICurrentUser _user = user;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ITransactionManager _transaction = transaction;

        public async Task<OrderResponse> Handle(CancelOrderCommand request, CancellationToken ct)
        {
            ValidateCurrentUser();

            await _transaction.BeginTransactionAsync(ct);

            try
            {
                var order = await _orders.GetByIdAsync(
                    request.OrderId, _user.UserId, _user.AnonymousId, ct)
                    ?? throw new NotFoundException(
                        "Order not found.");

                order.Cancel();

                var variantIds = order.Items
                    .Select(i => i.ProductVariantId)
                    .Distinct()
                    .ToArray();

                var products = await _products.GetByVariantIdsAsync(variantIds, ct);

                var variantsById = products
                    .SelectMany(p => p.Variants)
                    .Where(v => variantIds.Contains(v.Id))
                    .ToDictionary(v => v.Id);

                foreach (var item in order.Items)
                {
                    if (!variantsById.ContainsKey(item.ProductVariantId))
                    {
                        throw new NotFoundException(
                            $"Product variant {item.ProductVariantId} " +
                            "was not found while cancelling the order.");
                    }
                }

                foreach (var item in order.Items)
                {
                    var variant = variantsById[item.ProductVariantId];

                    variant.IncreaseStock(new StockQuantity(item.Quantity));
                }

                await _unitOfWork.SaveChangesAsync(ct);

                await _transaction.CommitTransactionAsync(ct);

                return await _queries.GetByIdAsync(
                order.Id,
                _user.UserId,
                _user.AnonymousId,
                ct)
                ?? throw new NotFoundException(
                    "The order could not be found.");
            }
            catch
            {
                await _transaction.RollbackTransactionAsync(CancellationToken.None);

                throw;
            }
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