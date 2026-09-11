using eCommerce.Domain.Catalog.Categories;
using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.Products.ProductValueObjects;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;

namespace eCommerce.Domain.Catalog.Products
{
    public class Product
    {
        private readonly List<Category> _categories = new();
        private readonly List<Option> _options = new();
        private readonly List<ProductVariant> _variants = new();
        public int Id { get; private set; }
        public ProductName Name { get; private set; }
        public ProductDescription Description { get; private set; }
        public IReadOnlyCollection<Category> Categories => _categories;
        public IReadOnlyCollection<Option> Options => _options;
        public IReadOnlyCollection<ProductVariant> Variants => _variants;
        public List<ProductImage> ProductImages { get; set; } = new();
        public List<ProductOptionValueImage> ProductOptionValueImages { get; set; } = new();
        public List<ProductCatalogAttribute> ProductCatalogAttributes { get; set; } = new();
        public Product(ProductName name)
        {
            Name = name;
            Description = ProductDescription.None;
        }
        private Product() { }

        public void Rename(ProductName name)
        {
            Name = name;
        }

        public void SetDescription(ProductDescription description)
        {
            Description = description;
        }
        public void RemoveDescription()
        {
            Description = ProductDescription.None;
        }

        public void AddCategory(Category category)
        {
            if (_categories.Contains(category)) throw new InvalidOperationException("Category already belongs to this product");
            _categories.Add(category);
        }
        public void RemoveCategory(Category category)
        {
            _categories.Remove(category);
        }
        public void AddOption(Option option)
        {
            if (_options.Contains(option)) throw new InvalidOperationException("Option already belongs to this product");
            _options.Add(option);
        }
        public void RemoveOption(Option option)
        {
            if (_variants.Any(v => v.OptionValues.Any(ov => option.Values.Contains(ov))))
                throw new InvalidOperationException(
                    "Cannot remove an option used by a product variant.");
            
            _options.Remove(option);
        }
        public void AddVariant(ProductVariant variant)
        {
            if (_variants.Any(v => v.Sku == variant.Sku)) throw new InvalidOperationException(
                $"Variant {variant.Sku} already belongs to this product");

            var optionValuesBelongToProduct = variant.OptionValues.All(variantOptionValue =>
                _options.Any(option => option.Values.Contains(variantOptionValue)));

            if (!optionValuesBelongToProduct) throw new InvalidOperationException(
                "Variant contains an option value that does not belong to this product");

            foreach (var option in _options)
            {
                var valuesForOption = variant.OptionValues
                    .Count(v => option.Values.Contains(v));

                if (valuesForOption == 0) throw new InvalidOperationException(
                    $"Variant is missing a value for option {option.Name}");

                if (valuesForOption > 1) throw new InvalidOperationException(
                    $"Variant contains multiple values for option {option.Name}");
            }

            _variants.Add(variant);
        }
        public void RemoveVariant(ProductVariant variant)
        {
            _variants.Remove(variant);
        }

        public void ChangeVariantSku(ProductVariant variant, ProductVariantSku sku)
        {
            if (_variants.Any(v => v != variant && v.Sku == sku))
                throw new InvalidOperationException($"Variant {sku} already belongs to this product.");
            
            variant.ChangeSku(sku);
        }

        public void ReplaceCategories(List<Category> categories)
        {
            var categoryIds = categories.Select(c => c.Id)
                .ToHashSet();

            foreach (var category in _categories.ToList())
            {
                if (!categories.Contains(category))
                    _categories.Remove(category);
            }

            foreach (var category in categories)
            {
                if (!_categories.Contains(category))
                    _categories.Add(category);
            }
        }

        public void ReplaceOptions(List<Option> options)
        {
            var optionIds = options.Select(o => o.Id)
                .ToHashSet();

            foreach (var option in _options.ToList())
            {
                if (!options.Contains(option))
                    _options.Remove(option);
            }
            foreach (var option in options)
            {
                if (!_options.Contains(option))
                    _options.Add(option);
            }
        }

        public void AddVariant(ProductVariantSku sku, Money price, 
            StockQuantity stockQuantity, IEnumerable<OptionValue> optionValues)
        {
            var variant = new ProductVariant(sku);

            variant.SetPrice(price);
            variant.SetStockQuantity(stockQuantity);

            foreach (var optionValue in optionValues)
            {
                variant.AddOptionValue(optionValue);
            }

            AddVariant(variant);
        }

        public void UpdateVariant(int variantId, ProductVariantSku sku, Money price, 
            StockQuantity stockQuantity, IEnumerable<OptionValue> optionValues)
        {
            var variant = _variants.FirstOrDefault(v => v.Id == variantId);

            if (variant == null) throw new InvalidOperationException(
                    $"Variant {variantId} does not belong to this product.");

            variant.ChangeSku(sku);
            variant.SetPrice(price);
            variant.SetStockQuantity(stockQuantity);
            variant.ReplaceOptionValues(optionValues);
        }

        public void RemoveVariantsNotIn(IEnumerable<int> variantIds)
        {
            var idsToKeep = variantIds.ToHashSet();

            foreach (var id in idsToKeep)
            {
                if (!_variants.Any(v => v.Id == id)) throw new InvalidOperationException(
                    $"Variant {id} does not belong to this product.");
                
            }

            foreach (var variant in _variants.ToList())
            {
                if (!idsToKeep.Contains(variant.Id))
                    _variants.Remove(variant);
            }
        }
    }
}
