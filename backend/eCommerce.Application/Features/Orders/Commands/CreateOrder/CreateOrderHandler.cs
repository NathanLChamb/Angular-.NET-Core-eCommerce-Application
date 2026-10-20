using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Carts.ValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ValueObjects;
using eCommerce.Domain.Order;
using eCommerce.Domain.Order.ValueObjects;
using MediatR;

namespace eCommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler(
        IOrderRepository orders,
        IOrderQueries queries,
        ICartRepository carts,
        IProductRepository products,
        ICurrentUser user,
        IUnitOfWork unitOfWork,
        ITransactionManager transaction) 
        : IRequestHandler<CreateOrderCommand, OrderResponse>
    {
        private readonly IOrderRepository _orders = orders;
        private readonly IOrderQueries _queries = queries;
        private readonly ICartRepository _carts = carts;
        private readonly IProductRepository _products = products;
        private readonly ICurrentUser _user = user;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ITransactionManager _transaction = transaction;

        public async Task<OrderResponse> Handle(
            CreateOrderCommand request, 
            CancellationToken ct)
        {
            ValidateCurrentUser();

            await _transaction.BeginTransactionAsync(ct);

            try
            {
                var cart = await _carts.GetAsync(_user.UserId, _user.AnonymousId, ct)
                    ?? throw new NotFoundException(
                        "Cart not found.");

                if (cart.Items.Count == 0) throw new BusinessRuleException(
                    "Cannot checkout an empty cart.");

                var variantIds = cart.Items
                    .Select(i => i.ProductVariantId)
                    .Distinct()
                    .ToArray();

                var products = await _products.GetByVariantIdsAsync(variantIds, ct);

                var variantsById = products
                    .SelectMany(p => p.Variants)
                    .Where(v => variantIds.Contains(v.Id))
                    .ToDictionary(v => v.Id);

                var productByVariantId = products
                    .SelectMany(p => p.Variants.Select(v => new
                    {
                        VariantId = v.Id,
                        Product = p
                    }))
                    .Where(x => variantIds.Contains(x.VariantId))
                    .ToDictionary(x => x.VariantId, x => x.Product);

                var orderItems = new List<OrderItemData>();

                foreach (var cartItem in cart.Items)
                {
                    if (!variantsById.TryGetValue(cartItem.ProductVariantId, out var variant) ||
                        !productByVariantId.TryGetValue(cartItem.ProductVariantId, out var product))
                    {
                        throw new NotFoundException(
                            $"Product variant {cartItem.ProductVariantId} was not found.");
                    }

                    if (variant.StockQuantity.Value < cartItem.Quantity)
                    {
                        throw new BusinessRuleException(
                            $"Insufficient stock for {variant.Sku.Value}.");
                    }

                    var imageUrl = variant.Pictures.Count > 0
                        ? variant.Pictures[0].Location.ToString()
                        : null;
                    var snapshot = new ProductSnapshot(product.Name.Value, variant.Sku.Value, imageUrl);
                    orderItems.Add(new OrderItemData(variant.Id, snapshot,
                        new Price(variant.Price.Amount), cartItem.Quantity));
                }

                var order = Order.Create(_user.UserId, _user.AnonymousId,
                    new OrderNumber(GenerateOrderNumber()), new ShippingAddress(request.ShippingAddress), orderItems);

                foreach (var cartItem in cart.Items)
                {
                    var variant = variantsById[cartItem.ProductVariantId];

                    variant.DecreaseStock(new StockQuantity(cartItem.Quantity));
                }

                await _orders.AddAsync(order, ct);

                cart.Clear();

                await _unitOfWork.SaveChangesAsync(ct);

                await _transaction.CommitTransactionAsync(ct);

                return await _queries.GetByIdAsync(order.Id, _user.UserId, _user.AnonymousId, ct)
                    ?? throw new InvalidOperationException(
                        "The order was saved but could not be retrieved.");
            }
            catch
            {
                await _transaction.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid()
                .ToString()[..8]
                .ToUpper()}";
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
