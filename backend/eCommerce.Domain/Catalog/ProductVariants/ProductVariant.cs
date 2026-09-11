using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;

namespace eCommerce.Domain.Catalog.ProductVariants
{
    public class ProductVariant
    {
        private readonly List<OptionValue> _optionValues = new();
        public int Id { get; private set; }
        public uint Version { get; set; }
        public ProductVariantSku Sku { get; private set; }
        public StockQuantity StockQuantity { get; private set; }
        public Money? Price { get; private set; }
        public IReadOnlyCollection<OptionValue> OptionValues => _optionValues;
        public ProductVariant(ProductVariantSku sku)
        {
            Sku = sku;
            StockQuantity = new StockQuantity(0);
        }
        private ProductVariant() { }

        public void AddOptionValue(OptionValue optionValue)
        {
            if (_optionValues.Contains(optionValue)) throw new InvalidOperationException(
                "Option value is already assigned to this variant");

            _optionValues.Add(optionValue);
        }
        public void RemoveOptionValue(OptionValue optionValue)
        {
            _optionValues.Remove(optionValue);
        }
        public void ReplaceOptionValues(IEnumerable<OptionValue> optionValues)
        {
            _optionValues.Clear();

            foreach (var optionValue in optionValues)
            {
                AddOptionValue(optionValue);
            }
        }
        public void SetStockQuantity(StockQuantity quantity)
        {
            StockQuantity = quantity;
        }
        public void IncreaseStock(StockQuantity quantity)
        {
            StockQuantity = StockQuantity.Add(quantity);
        }
        public void DecreaseStock(StockQuantity quantity) 
        {
            StockQuantity = StockQuantity.Subtract(quantity);
        }
        public void SetPrice(Money price)
        {
            Price = price;
        }

        public void ChangeSku(ProductVariantSku sku)
        {
            Sku = sku;
        }
    }
}
