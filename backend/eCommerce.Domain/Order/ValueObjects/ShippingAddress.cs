namespace eCommerce.Domain.Order.ValueObjects
{
    public record ShippingAddress
    {
        public string Value { get; private set; }

        public ShippingAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(
                    "Shipping address is required.",
                    nameof(value));

            value = value.Trim();

            if (value.Length > 500)
                throw new ArgumentException(
                    "Shipping address cannot exceed 500 characters.",
                    nameof(value));

            Value = value;
        }

        private ShippingAddress()
        {
            Value = string.Empty;
        }

        public override string ToString() => Value;
    }
}
