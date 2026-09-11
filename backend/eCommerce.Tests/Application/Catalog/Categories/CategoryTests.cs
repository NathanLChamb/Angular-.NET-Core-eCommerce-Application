using eCommerce.Domain.Catalog.Categories;

namespace eCommerce.Tests.Catalog.Categories
{
    public class CategoryTests
    {
        [Fact]
        public void Constructor_WithValidName_CreatesCategory()
        {
            var category = new Category("TestName");
            Assert.Equal("TestName", category.Name);
        }
        [Fact]
        public void Constructor_WithEmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Category(""));
        }
        [Fact]
        public void Constructor_WithWhiteSpaceName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Category("      "));
        }
        [Fact]
        public void Rename_WithValidName_UpdatesName()
        {
            var category = new Category("TestName");
            category.Rename("ImprovedTestName");
            Assert.Equal("ImprovedTestName", category.Name);
        }
        [Fact]
        public void Rename_WithEmptyName_ThrowsArgumentException()
        {
            var category = new Category("TestName");
            Assert.Throws<ArgumentException>(() => category.Rename(""));
        }
        [Fact]
        public void Rename_WithWhiteSpaceName_ThrowsArgumentException()
        {
            var category = new Category("TestName");
            Assert.Throws<ArgumentException>(() => category.Rename("      "));
        }
    }
}
