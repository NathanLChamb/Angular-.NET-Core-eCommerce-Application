using eCommerce.Application.Features.Orders.Commands.CreateOrder;
using eCommerce.Domain.Cart;
using eCommerce.Infrastructure.Persistence;
using eCommerce.Tests.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using eCommerce.Domain.Catalog.Products;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.Products.ProductValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;

namespace eCommerce.Tests.Application.Orders
{
    [Collection("Database Collection")]
    public class OrderCommandTests
    {
        private readonly PostgresContainerFixture _fixture;

        public OrderCommandTests(PostgresContainerFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public async Task CreateOrder_WithValidCart_ShouldCreateOrderAndClearCart()
        {
            await _fixture.ResetDatabase();

            using var scope = _fixture.Services.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var context = scope.ServiceProvider.GetRequiredService<eCommerceContext>();

            var product = new Product(new ProductName("Phone"));
            product.SetDescription(new ProductDescription("Test phone"));

            var variant = new ProductVariant(new ProductVariantSku("PHONE-1"));
            variant.SetPrice(new Money(500));
            variant.SetStockQuantity(new StockQuantity(10));

            product.AddVariant(variant);


            context.Products.Add(product);
            context.ProductVariants.Add(variant);

            await context.SaveChangesAsync();


            context.Carts.Add(new Domain.Cart.Cart
            {
                UserId = "user-1",
                Items =
        {
            new CartItem
            {
                ProductVariantId = variant.Id,
                ProductVariant = variant,
                Quantity = 2
            }
        }
            });

            await context.SaveChangesAsync();


            var result = await mediator.Send(
                new CreateOrderCommand(
                    "user-1",
                    "123 Test Street")
            );


            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.TotalPrice.Should().Be(1000);


            var cart = await context.Carts
                .Include(c => c.Items)
                .FirstAsync();

            cart.Items.Should().BeEmpty();


            var updatedVariant =
                await context.ProductVariants
                    .FirstAsync();

            updatedVariant.StockQuantity.Should().Be(8);
        }
        [Fact]
        public async Task CancelOrder_ShouldRestoreStock()
        {

        }
        [Fact]
        public async Task CancelOrder_WhenAlreadyShipped_ShouldFail()
        {

        }
        [Fact]
        public async Task UpdateOrderStatus_ShouldChangeStatus()
        {

        }
        [Fact]
        public async Task UpdateDeliveredOrder_ShouldFail()
        {

        }
        [Fact]
        public async Task UserCannotAccessOtherUsersOrder()
        {

        }
    }
}
