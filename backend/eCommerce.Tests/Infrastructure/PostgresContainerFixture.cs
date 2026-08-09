using eCommerce.Application.Features.Categories.Commands.CreateCategory;
using eCommerce.Application.Features.Products.Interfaces;
using eCommerce.Application.Features.Products.Services;
using eCommerce.Application.Interfaces;
using eCommerce.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace eCommerce.Tests.Infrastructure
{
    public sealed class PostgresContainerFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:latest")
                .WithDatabase("ecommerce_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .WithCleanUp(true)
                .Build();

        private DbConnection _connection = null!;
        private Respawner _respawner = null!;
        private DbContextOptions<eCommerceContext> _options = null!;
        public IServiceProvider Services { get; private set; } = null!;

        public string ConnectionString => _container.GetConnectionString();

        public async Task InitializeAsync()
        {
            await _container.StartAsync();

            _options = new DbContextOptionsBuilder<eCommerceContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            var services = new ServiceCollection();

            services.AddDbContext<eCommerceContext>(options =>
                options.UseNpgsql(ConnectionString));

            services.AddScoped<IeCommerceContext>(sp =>
                sp.GetRequiredService<eCommerceContext>());

            services.AddScoped<IProductValidationService, ProductValidationService>();

            services.AddLogging();

            services.AddScoped<ITransactionManager, TransactionManager>();

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(CreateCategoryCommand).Assembly));

            Services = services.BuildServiceProvider();

            _connection = new NpgsqlConnection(ConnectionString);
            await _connection.OpenAsync();

            using var scope = Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<eCommerceContext>();

            await context.Database.MigrateAsync();

            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" }
            });
        }

        public async Task ResetDatabase()
        {
            await _respawner.ResetAsync(_connection);
        }

        public eCommerceContext CreateDbContext()
        {
            return new eCommerceContext(_options);
        }

        public async Task DisposeAsync()
        {
            await _connection.DisposeAsync();
            await _container.DisposeAsync();
        }
    }
}
