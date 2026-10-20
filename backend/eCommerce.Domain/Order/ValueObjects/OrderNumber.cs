namespace eCommerce.Domain.Order.ValueObjects
{
    public record OrderNumber
    {
        public string Value { get; private set; }

        public OrderNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(
                    "Order number is required.",
                    nameof(value));

            value = value.Trim().ToUpperInvariant();

            if (value.Length > 32)
                throw new ArgumentException(
                    "Order number cannot exceed 32 characters.",
                    nameof(value));

            Value = value;
        }

        private OrderNumber()
        {
            Value = string.Empty;
        }

        public override string ToString() => Value;
    }
}