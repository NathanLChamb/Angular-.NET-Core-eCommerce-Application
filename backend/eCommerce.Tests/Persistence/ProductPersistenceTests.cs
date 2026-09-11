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
    public class ProductPersistenceTests
    {
        private readonly PostgresContainerFixture _fixture;
        public ProductPersistenceTests(PostgresContainerFixture fixture)
        {
            _fixture = fixture;
        }        

        [Fact]
        public async Task DifferentProducts_CanHaveSameSku()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var product1 = new Product(
                new ProductName("Product One"));

            var product2 = new Product(
                new ProductName("Product Two"));

            product1.AddVariant(
                new ProductVariant(
                    new ProductVariantSku("TEST-001")));

            product2.AddVariant(
                new ProductVariant(
                    new ProductVariantSku("TEST-001")));

            context.Products.AddRange(product1, product2);

            await context.SaveChangesAsync();

            Assert.Equal(2, await context.ProductVariants.CountAsync());
        }

        [Fact]
        public async Task Product_WithCategory_PersistsRelationship()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var product = new Product(
                new ProductName("Test Product"));

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
                new ProductName("Test Product"));

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
                new ProductName("Test Shirt"));

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
                new ProductName("Test Shirt"));

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

            var product = new Product(new ProductName("Test TV"));

            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));

            product.AddVariant(variant);

            context.Products.Add(product);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var result = await context.Products
                .Include(p => p.Variants)
                .SingleAsync(p => p.Id == product.Id);

            Assert.Single(result.Variants);
            Assert.Equal("TEST-TV-REMOTE", result.Variants.First().Sku.Value);
        }

        [Fact]
        public void AddVariant_WhenOptionValueDoesNotBelongToProductOption_ThrowsInvalidOperationException()
        {
            var product = new Product(new ProductName("Test Shirt"));

            var color = new Option("Color");
            var size = new Option("Size");

            var red = new OptionValue("Red");
            var large = new OptionValue("Large");

            color.AddValue(red);
            size.AddValue(large);

            product.AddOption(color);

            var variant = new ProductVariant(new ProductVariantSku("SHIRT-RED"));

            variant.AddOptionValue(large);

            Assert.Throws<InvalidOperationException>(
                () => product.AddVariant(variant));
        }

        [Fact]
        public void AddVariant_WhenOptionValueBelongsToProductOption_AddsVariant()
        {
            var product = new Product(
                new ProductName("Test Shirt"));

            var color = new Option("Color");
            var red = new OptionValue("Red");

            color.AddValue(red);
            product.AddOption(color);

            var variant = new ProductVariant(
                new ProductVariantSku("SHIRT-RED"));

            variant.AddOptionValue(red);

            product.AddVariant(variant);

            Assert.Contains(variant, product.Variants);
        }

        [Fact]
        public async Task DeletingProduct_CascadeDeletesVariants()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var product = new Product(
                new ProductName("Test Product"));

            var variant = new ProductVariant(
                new ProductVariantSku("TEST-PRODUCT-001"));

            product.AddVariant(variant);

            context.Products.Add(product);

            await context.SaveChangesAsync();

            context.Products.Remove(product);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var variantExists = await context.ProductVariants
                .AnyAsync(v => v.Id == variant.Id);

            Assert.False(variantExists);
        }
    }
}
