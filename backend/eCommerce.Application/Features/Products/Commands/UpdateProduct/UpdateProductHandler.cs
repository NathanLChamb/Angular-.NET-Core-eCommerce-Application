using eCommerce.Application.Exceptions;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Products.ProductValueObjects;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ValueObjects;
using MediatR;

namespace eCommerce.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductHandler(
        IProductRepository products,
        ICategoryRepository categories, 
        IOptionRepository options,
        IUnitOfWork unitOfWork) 
        : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _products = products;
        private readonly ICategoryRepository _categories = categories;
        private readonly IOptionRepository _options = options;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(
            UpdateProductCommand request, 
            CancellationToken ct)
        {
            var product = await _products.GetByIdAsync(request.Id, ct)
                ?? throw new NotFoundException(
                    "Product not found from provided ID.");

            var categoryIds = request.CategoryIds.Distinct().ToList();
            var categories = await _categories.GetByIdsAsync(categoryIds, ct);
            if (categories.Count != request.CategoryIds.Distinct().Count())
                throw new NotFoundException(
                    "One or more categories were not found");

            var optionIds = request.OptionIds.Distinct().ToList();
            var options = await _options.GetByIdsAsync(optionIds, ct);
            if (options.Count != request.OptionIds.Distinct().Count())
                throw new NotFoundException(
                    "One or more options were not found");

            var optionValuesById = options.SelectMany(o => o.Values)
                .ToDictionary(v => v.Id);

            product.Rename(new Name(request.Name));
            product.SetDescription(new Description(request.Description));

            product.UpdateCategories(categories);
            product.UpdateOptions(options);

            var existingVariantIds = request.ProductVariants
                .Where(v => v.Id.HasValue)
                .Select(v => v.Id!.Value)
                .ToHashSet();

            product.RemoveVariantsNotIn(existingVariantIds);

            foreach (var variantRequest in request.ProductVariants)
            {
                var optionValues = variantRequest.OptionValueIds
                    .Select(optionValueId =>
                    {
                        if (!optionValuesById.TryGetValue(optionValueId, out var optionValue))
                            throw new NotFoundException(
                                $"Option value with ID {optionValueId} was not found among the selected product options.");

                        return optionValue;
                    })
                    .ToList();

                var pictures = variantRequest.Pictures.Select(
                    picture => new Picture(
                        picture.Role, 
                        picture.Location))
                    .ToList();

                if (variantRequest.Id.HasValue)
                {
                    product.UpdateVariant(
                        variantRequest.Id.Value, 
                        new Sku(variantRequest.Sku), 
                        new Price(variantRequest.Price),
                        new StockQuantity(variantRequest.StockQuantity), 
                        optionValues, 
                        pictures);
                }
                else
                {
                    var variant = new ProductVariant(
                        new Sku(variantRequest.Sku), 
                        new Price(variantRequest.Price),
                        new StockQuantity(variantRequest.StockQuantity), 
                        optionValues, 
                        pictures);

                    product.AddVariant(variant);
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
