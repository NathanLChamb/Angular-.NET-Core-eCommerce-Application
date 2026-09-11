using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;

namespace eCommerce.Tests.Catalog.Products
{
    public class ProductVariantTests
    {
        [Fact]
        public void Constructor_WithValidSku_CreatesVariant()
        {
            var variantSku = new ProductVariantSku("TEST-TV-REMOTE");
            var variant = new ProductVariant(variantSku);
            Assert.Equal(variantSku, variant.Sku);
        }
        [Fact]
        public void Constructor_WithEmptySku_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ProductVariant(new ProductVariantSku("")));
        }
        [Fact]

        public void AddOptionValue_WithValidValue_AddsOptionValue()
        {
            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));
            var optionValue = new OptionValue("Red");
            variant.AddOptionValue(optionValue);

            Assert.Contains(optionValue, variant.OptionValues);
        }
        [Fact]
        public void AddOptionValue_WhenAlreadyExisting_ThrowsInvalidOperationException()
        {
            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));
            var optionValue = new OptionValue("Red");
            variant.AddOptionValue(optionValue);

            Assert.Throws<InvalidOperationException>(() => variant.AddOptionValue(optionValue));
        }
        [Fact]
        public void RemoveOptionValue_WhenValueExists_RemovesOptionValue()
        {
            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));
            var optionValue = new OptionValue("Red");

            variant.AddOptionValue(optionValue);
            variant.RemoveOptionValue(optionValue);

            Assert.DoesNotContain(optionValue, variant.OptionValues);
        }
        [Fact]

        public void SetStockQuantity_WithValidNumber_SetsStockQuantity()
        {
            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));
            var quantity = new StockQuantity(5);
            variant.SetStockQuantity(quantity);

            Assert.Equal(quantity, variant.StockQuantity);
        }

        [Fact]
        public void IncreaseStock_WithPositiveNumber_IncreasesStock()
        {
            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));
            variant.SetStockQuantity(new StockQuantity(10));
            var quantity = new StockQuantity(5);

            variant.IncreaseStock(quantity);

            Assert.Equal(15, variant.StockQuantity.Value);
        }

        [Fact]
        public void DecreaseStock_WithValidNumber_DecreasesStock()
        {
            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));
            variant.SetStockQuantity(new StockQuantity(10));
            var quantity = new StockQuantity(5);

            variant.DecreaseStock(quantity);

            Assert.Equal(5, variant.StockQuantity.Value);
        }

        [Fact]
        public void SetPrice_WithPositiveNumber_SetsPrice()
        {
            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));
            var price = new Money(5);

            variant.SetPrice(price);

            Assert.Equal(price, variant.Price);
        }
        [Fact]
        public void SetPrice_WithThreeDigits_ThrowsArgumentOutOfRangeException()
        {
            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));

            Assert.Throws<ArgumentOutOfRangeException>(() => variant.SetPrice(new Money(5.333m)));
        }
    }
}