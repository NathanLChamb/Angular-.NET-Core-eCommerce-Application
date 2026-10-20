using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ValueObjects;

namespace eCommerce.Domain.Catalog.ProductVariants
{
    public class ProductVariant
    {
        private readonly List<OptionValue> _optionValues = [];
        private readonly List<Picture> _pictures = [];
        public int Id { get; private set; }
        public uint Version { get; set; }
        public Sku Sku { get; private set; }
        public StockQuantity StockQuantity { get; private set; }
        public Price? Price { get; private set; }
        public IReadOnlyCollection<OptionValue> OptionValues => _optionValues;
        public IReadOnlyList<Picture> Pictures => _pictures;
        public ProductVariant(Sku sku, Price price, StockQuantity stockQuantity,
            IEnumerable<OptionValue> optionValues, IEnumerable<Picture> pictures)
        {
            Sku = sku;
            Price = price;
            StockQuantity = stockQuantity;

            foreach (var optionValue in optionValues)
            {
                AddOptionValue(optionValue);
            }

            _pictures.AddRange(pictures);
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
        public void SetPrice(Price price)
        {
            Price = price;
        }

        public void ChangeSku(Sku sku)
        {
            Sku = sku;
        }

        public void AddPicture(Picture picture)
        {
            _pictures.Add(picture);
        }

        public void Update(Sku sku, Price price, StockQuantity stockQuantity,
            IEnumerable<OptionValue> optionValues, IEnumerable<Picture> pictures)
        {
            Sku = sku;
            Price = price;
            StockQuantity = stockQuantity;

            _optionValues.Clear();
            foreach (var optionValue in optionValues)
            {
                AddOptionValue(optionValue);
            }

            _pictures.Clear();
            _pictures.AddRange(pictures);
        }
    }
}
