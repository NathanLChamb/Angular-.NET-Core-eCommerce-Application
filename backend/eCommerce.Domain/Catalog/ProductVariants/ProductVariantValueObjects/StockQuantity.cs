namespace eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects
{
    public record StockQuantity
    {
        public int Value { get; }
        public StockQuantity(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(
                nameof(value), "Stock cannot be less than 0");

            Value = value;
        }

        public StockQuantity Add(StockQuantity quantity)
        {
            return new StockQuantity(Value + quantity.Value);
        }
        public StockQuantity Subtract(StockQuantity quantity)
        {
            return new StockQuantity(Value - quantity.Value);
        }
    }
}
