using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.Products;
using eCommerce.Domain.Catalog.Products.ProductValueObjects;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using eCommerce.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Tests.Persistence
{
    [Collection("Database Collection")]
    public class ProductVariantPersistenceTests
    {
        private readonly PostgresContainerFixture _fixture;
        public ProductVariantPersistenceTests(PostgresContainerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void DecreaseStock_WhenQuantityWouldGoBelowZero_ThrowsArgumentOutOfRangeException()
        {
            var variant = new ProductVariant(
                new ProductVariantSku("TEST-TV-REMOTE"));

            variant.SetStockQuantity(new StockQuantity(5));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                variant.DecreaseStock(new StockQuantity(6)));
        }

        [Fact]
        public async Task ProductVariant_WithOptionValues_PersistsRelationship()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var option = new Option("Color");

            var red = new OptionValue("Red");
            var blue = new OptionValue("Blue");

            option.AddValue(red);
            option.AddValue(blue);

            var product = new Product(new ProductName("Test Shirt"));

            product.AddOption(option);

            var variant = new ProductVariant(new ProductVariantSku("TEST-SHIRT-RED"));

            variant.AddOptionValue(red);

            product.AddVariant(variant);

            context.Options.Add(option);
            context.Products.Add(product);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var result = await context.ProductVariants
                .Include(v => v.OptionValues)
                .SingleAsync(v => v.Id == variant.Id);

            Assert.Single(result.OptionValues);
            Assert.Equal("Red", result.OptionValues.First().Value);
        }

        [Fact]
        public async Task DeletingVariant_DeletesOptionValueRelationship()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var option = new Option("Color");

            var red = new OptionValue("Red");
            var blue = new OptionValue("Blue");

            option.AddValue(red);
            option.AddValue(blue);

            var product = new Product(new ProductName("Test Shirt"));

            product.AddOption(option);

            var variant = new ProductVariant(
                new ProductVariantSku("TEST-SHIRT-RED"));

            variant.AddOptionValue(red);
            product.AddVariant(variant);

            context.Options.Add(option);
            context.Products.Add(product);

            await context.SaveChangesAsync();

            var variantId = variant.Id;
            var redId = red.Id;

            var relationshipBeforeDelete = await context.Database
                .SqlQuery<int>($"""
                    SELECT COUNT(*) AS "Value"
                    FROM "ProductVariantOptionValues"
                    WHERE "ProductVariantId" = {variantId}
                      AND "OptionValueId" = {redId}
                    """)
                .SingleAsync();

            Assert.Equal(1, relationshipBeforeDelete);

            context.ProductVariants.Remove(variant);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var variantExists = await context.ProductVariants
                .AnyAsync(v => v.Id == variantId);

            var redExists = await context.OptionValues
                .AnyAsync(v => v.Id == redId);

            var relationshipCount = await context.Database
                .SqlQuery<int>($"""
                    SELECT COUNT(*) AS "Value"
                    FROM "ProductVariantOptionValues"
                    WHERE "ProductVariantId" = {variantId}
                      AND "OptionValueId" = {redId}
                    """)
                .SingleAsync();

            Assert.False(variantExists);
            Assert.True(redExists);
            Assert.Equal(0, relationshipCount);
        }
    }
}
