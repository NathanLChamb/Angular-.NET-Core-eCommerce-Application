using eCommerce.Domain.Catalog.Categories;
using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.Products.ProductValueObjects;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ValueObjects;

namespace eCommerce.Domain.Catalog.Products
{
    public class Product
    {
        private readonly List<Category> _categories = [];
        private readonly List<Option> _options = [];
        private readonly List<ProductVariant> _variants = [];
        public int Id { get; private set; }
        public Name Name { get; private set; }
        public Description Description { get; private set; }
        public IReadOnlyCollection<Category> Categories => _categories;
        public IReadOnlyCollection<Option> Options => _options;
        public IReadOnlyCollection<ProductVariant> Variants => _variants;
        public List<ProductCatalogAttribute> ProductCatalogAttributes { get; set; } = [];
        public Product(Name name)
        {
            Name = name;
            Description = Description.None;
        }
        private Product() { }

        public void Rename(Name name)
        {
            Name = name;
        }

        public void SetDescription(Description description)
        {
            Description = description;
        }
        public void RemoveDescription()
        {
            Description = Description.None;
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

        public void ChangeVariantSku(ProductVariant variant, Sku sku)
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
        public void AddCategories(IEnumerable<Category> categories)
        {
            _categories.AddRange(categories);
        }
        public void UpdateCategories(IEnumerable<Category> categories)
        {
            var newCategories = categories.ToList();
            var newCategoryIds = newCategories
                .Select(c => c.Id)
                .ToHashSet();

            _categories.RemoveAll(c => !newCategoryIds.Contains(c.Id));

            foreach (var category in newCategories)
            {
                if (!_categories.Any(c => c.Id == category.Id))
                    _categories.Add(category);
            }
        }
        public void AddOptions(IEnumerable<Option> options)
        {
            _options.AddRange(options);
        }
        public void UpdateOptions(IEnumerable<Option> options)
        {
            var newOptions = options.ToList();
            var newOptionIds = newOptions
                .Select(o => o.Id)
                .ToHashSet();

            _options.RemoveAll(o => !newOptionIds.Contains(o.Id));

            foreach (var option in newOptions)
            {
                if (!_options.Any(o => o.Id == option.Id))
                    _options.Add(option);
            }
        }

        public void AddVariants(IEnumerable<ProductVariant> variants)
        {
            _variants.AddRange(variants);
        }
        public void UpdateVariant(int variantId, Sku sku, Price price, StockQuantity stockQuantity,
            IEnumerable<OptionValue> optionValues, IEnumerable<Picture> pictures)
        {
            var variant = _variants.FirstOrDefault(v => v.Id == variantId)
                ?? throw new ArgumentException(
                    $"Variant with ID {variantId} does not belong to this product.",
                    nameof(variantId));

            variant.Update(sku, price, stockQuantity, optionValues, pictures);
        }

        public void RemoveVariantsNotIn(ISet<int> variantIds)
        {
            _variants.RemoveAll(v => !variantIds.Contains(v.Id));
        }
    }
}
