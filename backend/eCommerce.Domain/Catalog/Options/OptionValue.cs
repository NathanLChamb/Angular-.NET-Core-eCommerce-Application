using eCommerce.Domain.Catalog.Products;

namespace eCommerce.Domain.Catalog.Options
{
    public class OptionValue
    {
        public int Id { get; private set; }
        public string Value { get; private set; }
        public List<ProductOptionValueImageOptionValue> ProductOptionValues { get; set; } = new();
        public OptionValueStatus Status { get; private set; }
        public OptionValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Option value cannot be empty", nameof(value));

            Value = value.Trim();
        }
        internal OptionValue(int id, string value)
        {
            Id = id;
            Value = value;
        }
        internal void Rename(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Option value cannot be empty", nameof(value));

            Value = value.Trim();
        }
    }
}
