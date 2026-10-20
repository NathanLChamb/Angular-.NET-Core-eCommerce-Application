using eCommerce.Domain.Catalog.Categories;
using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.Products;
using eCommerce.Domain.Catalog.Products.ProductValueObjects;

namespace eCommerce.Tests.Application.Catalog.Products
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_WithValidName_CreatesProduct()
        {
            var name = new Name("TestName");
            var product = new Product(name);
            Assert.Equal(name, product.Name);
        }
        [Fact]
        public void Constructor_SetDescriptionToNone_CreatesProduct()
        {
            var product = new Product(new Name("TestName"));

            Assert.Equal(Description.None, product.Description);
        }
        [Fact]
        public void Constructor_WithDescriptionOver300Characters_ThrowsArgumentOutOfRangeException()
        {
            var description = new string('A', 301);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Description(description));
        }
        [Fact]
        public void SetDescription_WithValidDescription_UpdatesDescription()
        {
            var product = new Product(new Name("TestName"));
            var description = new Description("Test description for verification");
            product.SetDescription(description);

            Assert.Equal(description, product.Description);
        }
        
        [Fact]
        public void RemoveDescription_WithExistingDescription_RemovesDescription()
        {
            var product = new Product(new Name("TestName"));
            product.SetDescription(new Description("Test Description"));
            product.RemoveDescription();

            Assert.Equal(Description.None, product.Description);
        }

        [Fact]
        public void AddCategory_WithValidCategory_CreatesCategory()
        {
            var product = new Product(new Name("TestName"));
            var category = new Category("Electronics");

            product.AddCategory(category);

            Assert.Contains(category, product.Categories);
        }
        [Fact]
        public void AddCategory_WhenAlreadyExists_ThrowsInvalidOperationException()
        {
            var product = new Product(new Name("TestName"));
            var category = new Category("Electronics");
            product.AddCategory(category);

            Assert.Throws<InvalidOperationException>(() => product.AddCategory(category));
        }
        [Fact]
        public void RemoveCategory_WhenCategoryExists_RemovesCategory()
        {
            var product = new Product(new Name("TestName"));
            var category = new Category("Electronics");

            product.AddCategory(category);
            product.RemoveCategory(category);

            Assert.DoesNotContain(category, product.Categories);
        }

        [Fact]
        public void AddOption_WithValidOption_CreatesOption()
        {
            var product = new Product(new Name("TestName"));
            var option = new Option("Color");

            product.AddOption(option);

            Assert.Contains(option, product.Options);
        }
        [Fact]
        public void AddOption_WhenAlreadyExists_ThrowsInvalidOperationException()
        {
            var product = new Product(new Name("TestName"));
            var option = new Option("Color");

            product.AddOption(option);

            Assert.Throws<InvalidOperationException>(() => product.AddOption(option));
        }
        [Fact]
        public void RemoveOption_WhenOptionExists_RemovesOption()
        {
            var product = new Product(new Name("TestName"));
            var option = new Option("Color");

            product.AddOption(option);
            product.RemoveOption(option);

            Assert.DoesNotContain(option, product.Options);
        }

        [Fact]
        public void AddVariant_WithValidVariant_AddsVariant()
        {
            
        }
        [Fact]
        public void AddVariant_WhenAlreadyExists_ThrowsInvalidOperationException()
        {
            
        }
        [Fact]
        public void RemoveVariant_WhenExisting_RemovesVariant()
        {
            
        }

        [Fact]
        public void Product_WithVariantsContainingValueForEveryOption_AddsVariants()
        {
            
        }
        [Fact]
        public void Product_AddVariant_WhenVariantIsMissingOptionValue_ThrowsInvalidOperationException()
        {
            
        }

        [Fact]
        public void Product_AddVariants_WithMultipleValuesFromOneOption_ThrowsInvalidOperationException()
        {
            
        }
    }
}