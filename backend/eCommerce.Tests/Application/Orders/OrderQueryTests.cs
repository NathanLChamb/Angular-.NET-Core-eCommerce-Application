using eCommerce.Infrastructure.Persistence;
using eCommerce.Tests.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Tests.Application.Orders
{
    [Collection("Database Collection")]
    public class OrderQueryTests(PostgresContainerFixture fixture)
    {
        private readonly PostgresContainerFixture _fixture = fixture;

        [Fact]
        public async Task GetMyOrders_ShouldReturnOnlyUsersOrders()
        {
            await _fixture.ResetDatabase();

            using var scope = _fixture.Services.CreateScope();

            var mediator =
                scope.ServiceProvider.GetRequiredService<IMediator>();

            var context =
                scope.ServiceProvider.GetRequiredService<eCommerceContext>();

        }
    }
}