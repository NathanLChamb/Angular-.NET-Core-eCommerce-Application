using eCommerce.Domain.Catalog.Categories;
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
    public class ProductPersistenceTests(
        PostgresContainerFixture fixture)
    {
        private readonly PostgresContainerFixture _fixture = fixture;  

        [Fact]
        public async Task DifferentProducts_CanHaveSameSku()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();
        }

        [Fact]
        public async Task Product_WithCategory_PersistsRelationship()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var product = new Product(
                new Name("Test Product"));

            var category = new Category("Electronics");

            product.AddCategory(category);

            context.Products.Add(product);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var result = await context.Products
                .Include(p => p.Categories)
                .SingleAsync(p => p.Id == product.Id);

            Assert.Single(result.Categories);
            Assert.Equal("Electronics", result.Categories.First().Name);
        }
        [Fact]
        public async Task DeletingProduct_DeletesCategoryRelationship()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var product = new Product(
                new Name("Test Product"));

            var category = new Category("Electronics");

            product.AddCategory(category);

            context.Products.Add(product);

            await context.SaveChangesAsync();

            var productId = product.Id;
            var categoryId = category.Id;

            var relationshipBeforeDelete = await context.Database
                .SqlQuery<int>($"""
                    SELECT COUNT(*) AS "Value"
                    FROM "ProductCategories"
                    WHERE "ProductId" = {productId}
                        AND "CategoryId" = {categoryId}
                    """)
                .SingleAsync();

            Assert.Equal(1, relationshipBeforeDelete);

            context.Products.Remove(product);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var categoryExists = await context.Categories
                .AnyAsync(c => c.Id == categoryId);

            var productExists = await context.Products
                .AnyAsync(p => p.Id == productId);

            var relationshipExists = await context.Database
                .SqlQuery<int>($"""
                    SELECT COUNT(*) AS "Value"
                    FROM "ProductCategories"
                    WHERE "ProductId" = {productId}
                        AND "CategoryId" = {categoryId}
                    """)
                .SingleAsync();

            Assert.False(productExists);
            Assert.True(categoryExists);
            Assert.Equal(0, relationshipExists);
        }

        [Fact]
        public async Task Product_WithOption_PersistsRelationship()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var product = new Product(
                new Name("Test Shirt"));

            var option = new Option("Color");

            product.AddOption(option);

            context.Products.Add(product);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var result = await context.Products
                .Include(p => p.Options)
                .SingleAsync(p => p.Id == product.Id);

            Assert.Single(result.Options);
            Assert.Equal("Color", result.Options.First().Name);
        }
        [Fact]
        public async Task DeletingProduct_DeletesOptionRelationship()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var product = new Product(
                new Name("Test Shirt"));

            var option = new Option("Color");

            product.AddOption(option);

            context.Products.Add(product);

            await context.SaveChangesAsync();

            var productId = product.Id;
            var optionId = option.Id;

            var relationshipBeforeDelete = await context.Database
                .SqlQuery<int>($"""
                    SELECT COUNT(*) AS "Value"
                    FROM "ProductOptions"
                    WHERE "ProductId" = {productId}
                        AND "OptionId" = {optionId}
                    """)
                .SingleAsync();

            Assert.Equal(1, relationshipBeforeDelete);

            context.Products.Remove(product);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var productExists = await context.Products
                .AnyAsync(p => p.Id == productId);

            var optionExists = await context.Options
                .AnyAsync(o => o.Id == optionId);

            var relationshipExists = await context.Database
                .SqlQuery<int>($"""
                    SELECT COUNT(*) AS "Value"
                    FROM "ProductOptions"
                    WHERE "ProductId" = {productId}
                        AND "OptionId" = {optionId}
                    """)
                .SingleAsync();

            Assert.False(productExists);
            Assert.True(optionExists);
            Assert.Equal(0, relationshipExists);
        }

        [Fact]
        public async Task Product_WithVariant_PersistsRelationship()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();
        }

        [Fact]
        public void AddVariant_WhenOptionValueDoesNotBelongToProductOption_ThrowsInvalidOperationException()
        {

        }

        [Fact]
        public void AddVariant_WhenOptionValueBelongsToProductOption_AddsVariant()
        {
            
        }

        [Fact]
        public async Task DeletingProduct_CascadeDeletesVariants()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();
        }
    }
}
