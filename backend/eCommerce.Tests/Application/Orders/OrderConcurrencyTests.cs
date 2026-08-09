using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Orders.Commands.CreateOrder;
using eCommerce.Domain.Cart;
using eCommerce.Domain.Product;
using eCommerce.Tests.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Tests.Application.Orders
{
    [Collection("Database Collection")]
    public class ProductVariantConcurrencyTests
    {
        private readonly PostgresContainerFixture _fixture;

        public ProductVariantConcurrencyTests(PostgresContainerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Saving_stale_product_variant_throws_concurrency_exception()
        {
            await _fixture.ResetDatabase();

            // Arrange
            await using var setupContext = _fixture.CreateDbContext();

            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Product"
            };

            var variant = new ProductVariant
            {
                Sku = "TEST-SKU",
                Price = 10m,
                StockQuantity = 10,
                Product = product
            };

            setupContext.ProductVariants.Add(variant);
            await setupContext.SaveChangesAsync();

            var variantId = variant.Id;

            await using var contextA = _fixture.CreateDbContext();
            await using var contextB = _fixture.CreateDbContext();

            // Both contexts load the same database version.
            var variantA = await contextA.ProductVariants
                .SingleAsync(x => x.Id == variantId);

            var variantB = await contextB.ProductVariants
                .SingleAsync(x => x.Id == variantId);

            Assert.Equal(variantA.Version, variantB.Version);

            // Act
            variantA.StockQuantity -= 2;
            variantB.StockQuantity -= 3;

            // A wins.
            await contextA.SaveChangesAsync();

            // B is now stale and must fail.
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                () => contextB.SaveChangesAsync());

            // Assert
            await using var verificationContext = _fixture.CreateDbContext();

            var finalVariant = await verificationContext.ProductVariants
                .SingleAsync(x => x.Id == variantId);

            Assert.Equal(8, finalVariant.StockQuantity);
        }

        [Fact]
        public async Task Updating_same_product_variant_concurrently_throws_concurrency_exception()
        {
            await _fixture.ResetDatabase();

            // Arrange
            await using var setupContext = _fixture.CreateDbContext();

            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Product"
            };

            var variant = new ProductVariant
            {
                Sku = "TEST-SKU",
                Price = 10m,
                StockQuantity = 10,
                Product = product
            };

            setupContext.ProductVariants.Add(variant);
            await setupContext.SaveChangesAsync();

            var variantId = variant.Id;

            // Two completely independent EF contexts.
            await using var contextA = _fixture.CreateDbContext();
            await using var contextB = _fixture.CreateDbContext();

            // Both load the SAME database state.
            var variantA = await contextA.ProductVariants
                .SingleAsync(x => x.Id == variantId);

            var variantB = await contextB.ProductVariants
                .SingleAsync(x => x.Id == variantId);

            // Both should have observed the same xmin.
            Assert.Equal(variantA.Version, variantB.Version);

            // Act
            variantA.StockQuantity -= 2;
            variantB.StockQuantity -= 3;

            await contextA.SaveChangesAsync();

            // B still has the OLD Version/xmin.
            var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                () => contextB.SaveChangesAsync());

            // Assert
            Assert.NotNull(exception);

            await using var verificationContext = _fixture.CreateDbContext();

            var finalVariant = await verificationContext.ProductVariants
                .SingleAsync(x => x.Id == variantId);

            Assert.Equal(8, finalVariant.StockQuantity);
        }

        [Fact]
        public async Task Concurrent_checkouts_for_same_stock_allow_only_one_order()
        {
            await _fixture.ResetDatabase();

            // Arrange
            const string userA = "user-a";
            const string userB = "user-b";

            const int initialStock = 5;
            const int quantityPerCustomer = 3;

            await using var setupContext = _fixture.CreateDbContext();

            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Product"
            };

            var variant = new ProductVariant
            {
                Sku = "TEST-CONCURRENT-SKU",
                Price = 10m,
                StockQuantity = initialStock,
                Product = product
            };

            setupContext.ProductVariants.Add(variant);
            await setupContext.SaveChangesAsync();

            var variantId = variant.Id;

            var cartA = new Domain.Cart.Cart
            {
                UserId = userA
            };

            cartA.Items.Add(new CartItem
            {
                ProductVariantId = variantId,
                Quantity = quantityPerCustomer
            });

            var cartB = new Domain.Cart.Cart
            {
                UserId = userB
            };

            cartB.Items.Add(new CartItem
            {
                ProductVariantId = variantId,
                Quantity = quantityPerCustomer
            });

            setupContext.Carts.AddRange(cartA, cartB);
            await setupContext.SaveChangesAsync();

            // Each checkout gets its own scope and DbContext.
            using var scopeA = _fixture.Services.CreateScope();
            using var scopeB = _fixture.Services.CreateScope();

            var mediatorA = scopeA.ServiceProvider.GetRequiredService<IMediator>();
            var mediatorB = scopeB.ServiceProvider.GetRequiredService<IMediator>();

            var commandA = new CreateOrderCommand(
                UserId: userA,
                ShippingAddress: "123 Test Street"
            );

            var commandB = new CreateOrderCommand(
                UserId: userB,
                ShippingAddress: "456 Test Street"
            );

            // Act
            var taskA = Record.ExceptionAsync(
                () => mediatorA.Send(commandA));

            var taskB = Record.ExceptionAsync(
                () => mediatorB.Send(commandB));

            await Task.WhenAll(taskA, taskB);

            var exceptionA = await taskA;
            var exceptionB = await taskB;

            // Assert
            // Exactly one checkout succeeds.
            Assert.True(
                (exceptionA is null && exceptionB is ConflictException) ||
                (exceptionB is null && exceptionA is ConflictException));

            // Verify final database state.
            await using var verificationContext = _fixture.CreateDbContext();

            var finalVariant = await verificationContext.ProductVariants
                .SingleAsync(x => x.Id == variantId);

            // One customer purchased 3:
            // 5 - 3 = 2
            Assert.Equal(2, finalVariant.StockQuantity);

            // Exactly one order was created.
            var orders = await verificationContext.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();

            Assert.Single(orders);

            var order = orders.Single();

            Assert.Single(order.OrderItems);

            var orderItem = order.OrderItems.Single();

            Assert.Equal(variantId, orderItem.ProductVariantId);
            Assert.Equal(quantityPerCustomer, orderItem.Quantity);

            // The failed checkout must have been rolled back.
            // Therefore its cart should still contain the item.
            var losingUserId = exceptionA is ConflictException
                ? userA
                : userB;

            var losingCart = await verificationContext.Carts
                .Include(c => c.Items)
                .SingleAsync(c => c.UserId == losingUserId);

            Assert.Single(losingCart.Items);
            Assert.Equal(
                quantityPerCustomer,
                losingCart.Items.Single().Quantity);
        }
    }
}