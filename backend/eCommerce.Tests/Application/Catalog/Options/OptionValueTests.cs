using eCommerce.Domain.Catalog.Categories;
using eCommerce.Domain.Catalog.Options;

namespace eCommerce.Tests.Catalog.Options
{
    public class OptionValueTests
    {
        [Fact]
        public void Constructor_WithValidName_CreatesOptionValue()
        {
            var optionValue = new OptionValue("TestName");
            Assert.Equal("TestName", optionValue.Value);
        }
        [Fact]
        public void Constructor_WithEmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new OptionValue(""));
        }
        [Fact]
        public void Constructor_WithWhiteSpaceName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new OptionValue("      "));
        }
        [Fact]
        public void RenameValue_WithValidName_UpdatesName()
        {
            var option = new Option("TestOption");
            var optionValue = new OptionValue(1, "TestName");

            option.AddValue(optionValue);

            option.RenameValue(1, "ImprovedTestName");

            Assert.Equal("ImprovedTestName", optionValue.Value);
        }
        [Fact]
        public void RenameValue_WithEmptyName_ThrowsArgumentException()
        {
            var option = new Option("TestOption");
            var optionValue = new OptionValue(1, "TestName");
            option.AddValue(optionValue);

            Assert.Throws<ArgumentException>(() => option.RenameValue(1, ""));
        }
        [Fact]
        public void RenameValue_WithWhiteSpaceName_ThrowsArgumentException()
        {
            var option = new Option("TestOption");
            var optionValue = new OptionValue(1, "TestName");
            option.AddValue(optionValue);

            Assert.Throws<ArgumentException>(() => option.RenameValue(1, "      "));
        }
    }
}
