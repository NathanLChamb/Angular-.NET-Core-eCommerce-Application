using eCommerce.Application.Features.Orders.Commands.CreateOrder;
using eCommerce.Domain.Carts;
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
    public class OrderCommandTests(
        PostgresContainerFixture fixture)
    {
        private readonly PostgresContainerFixture _fixture = fixture;

        [Fact]
        public async Task CreateOrder_WithValidCart_ShouldCreateOrderAndClearCart()
        {

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
