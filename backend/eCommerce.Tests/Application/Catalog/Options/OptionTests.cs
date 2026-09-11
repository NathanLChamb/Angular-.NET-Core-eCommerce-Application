using eCommerce.Domain.Catalog.Options;

namespace eCommerce.Tests.Catalog.Options
{
    public class OptionTests
    {
        [Fact]
        public void Constructor_GivenValidName_CreatesOption()
        {
            var option = new Option("TestName");
            Assert.Equal("TestName", option.Name);
        }
        [Fact]
        public void Constructor_GivenEmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Option(""));
        }

        [Fact]
        public void AddValue_WithValidValue_AddsValue()
        {
            var option = new Option("Color");
            var value = new OptionValue("Blue");

            option.AddValue(value);

            Assert.Contains(value, option.Values);
        }
        [Fact]
        public void RemoveValue_WithExistingValue_RemovesValue()
        {
            var option = new Option("Color");
            var value = new OptionValue("Blue");

            option.AddValue(value);
            option.RemoveValue(value);

            Assert.DoesNotContain(value, option.Values);
        }
        [Fact]
        public void AddValue_WhenValueAlreadyExists_ThrowsInvalidOperationException()
        {
            var option = new Option("Color");

            option.AddValue(new OptionValue("Blue"));

            Assert.Throws<InvalidOperationException>(() => option.AddValue(new OptionValue("Blue")));
        }
    }
}
