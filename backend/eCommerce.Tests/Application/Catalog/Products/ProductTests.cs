using eCommerce.Domain.Catalog.Categories;
using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.Products;
using eCommerce.Domain.Catalog.Products.ProductValueObjects;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;

namespace eCommerce.Tests.Catalog.Products
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_WithValidName_CreatesProduct()
        {
            var name = new ProductName("TestName");
            var product = new Product(name);
            Assert.Equal(name, product.Name);
        }
        [Fact]
        public void Constructor_SetDescriptionToNone_CreatesProduct()
        {
            var product = new Product(new ProductName("TestName"));

            Assert.Equal(ProductDescription.None, product.Description);
        }
        [Fact]
        public void Constructor_WithDescriptionOver300Characters_ThrowsArgumentOutOfRangeException()
        {
            var description = new string('A', 301);

            Assert.Throws<ArgumentOutOfRangeException>(() => new ProductDescription(description));
        }
        [Fact]
        public void SetDescription_WithValidDescription_UpdatesDescription()
        {
            var product = new Product(new ProductName("TestName"));
            var description = new ProductDescription("Test description for verification");
            product.SetDescription(description);

            Assert.Equal(description, product.Description);
        }
        
        [Fact]
        public void RemoveDescription_WithExistingDescription_RemovesDescription()
        {
            var product = new Product(new ProductName("TestName"));
            product.SetDescription(new ProductDescription("Test Description"));
            product.RemoveDescription();

            Assert.Equal(ProductDescription.None, product.Description);
        }

        [Fact]
        public void AddCategory_WithValidCategory_CreatesCategory()
        {
            var product = new Product(new ProductName("TestName"));
            var category = new Category("Electronics");

            product.AddCategory(category);

            Assert.Contains(category, product.Categories);
        }
        [Fact]
        public void AddCategory_WhenAlreadyExists_ThrowsInvalidOperationException()
        {
            var product = new Product(new ProductName("TestName"));
            var category = new Category("Electronics");
            product.AddCategory(category);

            Assert.Throws<InvalidOperationException>(() => product.AddCategory(category));
        }
        [Fact]
        public void RemoveCategory_WhenCategoryExists_RemovesCategory()
        {
            var product = new Product(new ProductName("TestName"));
            var category = new Category("Electronics");

            product.AddCategory(category);
            product.RemoveCategory(category);

            Assert.DoesNotContain(category, product.Categories);
        }

        [Fact]
        public void AddOption_WithValidOption_CreatesOption()
        {
            var product = new Product(new ProductName("TestName"));
            var option = new Option("Color");

            product.AddOption(option);

            Assert.Contains(option, product.Options);
        }
        [Fact]
        public void AddOption_WhenAlreadyExists_ThrowsInvalidOperationException()
        {
            var product = new Product(new ProductName("TestName"));
            var option = new Option("Color");

            product.AddOption(option);

            Assert.Throws<InvalidOperationException>(() => product.AddOption(option));
        }
        [Fact]
        public void RemoveOption_WhenOptionExists_RemovesOption()
        {
            var product = new Product(new ProductName("TestName"));
            var option = new Option("Color");

            product.AddOption(option);
            product.RemoveOption(option);

            Assert.DoesNotContain(option, product.Options);
        }

        [Fact]
        public void AddVariant_WithValidVariant_AddsVariant()
        {
            var product = new Product(new ProductName("TestName"));
            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));

            product.AddVariant(variant);

            Assert.Contains(variant, product.Variants);
        }
        [Fact]
        public void AddVariant_WhenAlreadyExists_ThrowsInvalidOperationException()
        {
            var product = new Product(new ProductName("TestName"));
            var variant1 = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));
            var variant2 = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));

            product.AddVariant(variant1);

            Assert.Throws<InvalidOperationException>(() => product.AddVariant(variant2));
        }
        [Fact]
        public void RemoveVariant_WhenExisting_RemovesVariant()
        {
            var product = new Product(new ProductName("TestName"));
            var variant = new ProductVariant(new ProductVariantSku("TEST-TV-REMOTE"));

            product.AddVariant(variant);
            product.RemoveVariant(variant);

            Assert.DoesNotContain(variant, product.Variants);
        }

        [Fact]
        public void Product_WithVariantsContainingValueForEveryOption_AddsVariants()
        {
            var product = new Product(new ProductName("Test Couch"));

            var size = new Option("Size");
            var color = new Option("Color");

            var sizevalue1 = new OptionValue("small");
            var sizevalue2 = new OptionValue("large");
            var color1 = new OptionValue("red");
            var color2 = new OptionValue("blue");

            size.AddValue(sizevalue1);
            size.AddValue(sizevalue2);
            color.AddValue(color1);
            color.AddValue(color2);

            product.AddOption(size);
            product.AddOption(color);

            var smallRed = new ProductVariant(new ProductVariantSku("SMALL_RED_COUCH"));
            smallRed.AddOptionValue(sizevalue1);
            smallRed.AddOptionValue(color1);

            var largeRed = new ProductVariant(new ProductVariantSku("LARGE_RED_COUCH"));
            largeRed.AddOptionValue(sizevalue2);
            largeRed.AddOptionValue(color1);

            var largeBlue = new ProductVariant(new ProductVariantSku("LARGE_BLUE_COUCH"));
            largeBlue.AddOptionValue(sizevalue2);
            largeBlue.AddOptionValue(color2);

            var smallBlue = new ProductVariant(new ProductVariantSku("SMALL_BLUE_COUCH"));
            smallBlue.AddOptionValue(sizevalue1);
            smallBlue.AddOptionValue(color2);

            product.AddVariant(smallRed);
            product.AddVariant(largeRed);
            product.AddVariant(largeBlue);
            product.AddVariant(smallBlue);

            Assert.Equal(4, product.Variants.Count);
        }
        [Fact]
        public void Product_AddVariant_WhenVariantIsMissingOptionValue_ThrowsInvalidOperationException()
        {
            var product = new Product(new ProductName("Test Couch"));

            var size = new Option("Size");
            var color = new Option("Color");

            var sizevalue1 = new OptionValue("small");
            var sizevalue2 = new OptionValue("large");
            var color1 = new OptionValue("red");
            var color2 = new OptionValue("blue");

            size.AddValue(sizevalue1);
            size.AddValue(sizevalue2);
            color.AddValue(color1);
            color.AddValue(color2);

            product.AddOption(size);
            product.AddOption(color);

            var smallRed = new ProductVariant(new ProductVariantSku("SMALL_RED_COUCH"));
            smallRed.AddOptionValue(sizevalue1);
            smallRed.AddOptionValue(color1);

            var largeRed = new ProductVariant(new ProductVariantSku("LARGE_RED_COUCH"));
            largeRed.AddOptionValue(sizevalue2);
            largeRed.AddOptionValue(color1);

            var largeBlue = new ProductVariant(new ProductVariantSku("LARGE_BLUE_COUCH"));
            largeBlue.AddOptionValue(sizevalue2);
            largeBlue.AddOptionValue(color2);

            var smallBlue = new ProductVariant(new ProductVariantSku("SMALL_BLUE_COUCH"));
            smallBlue.AddOptionValue(sizevalue1);

            product.AddVariant(smallRed);
            product.AddVariant(largeRed);
            product.AddVariant(largeBlue);

            Assert.Throws<InvalidOperationException>(() => product.AddVariant(smallBlue));
        }

        [Fact]
        public void Product_AddVariants_WithMultipleValuesFromOneOption_ThrowsInvalidOperationException()
        {
            var product = new Product(new ProductName("Test Couch"));

            var size = new Option("Size");

            var sizevalue1 = new OptionValue("small");
            var sizevalue2 = new OptionValue("large");

            size.AddValue(sizevalue1);
            size.AddValue(sizevalue2);

            product.AddOption(size);

            var couch = new ProductVariant(new ProductVariantSku("LARGE-SMALL-COUCH"));
            couch.AddOptionValue(sizevalue1);
            couch.AddOptionValue(sizevalue2);

            Assert.Throws<InvalidOperationException>(() => product.AddVariant(couch));
        }
    }
}