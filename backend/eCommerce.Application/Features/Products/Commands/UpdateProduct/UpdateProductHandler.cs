using eCommerce.Application.Exceptions;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Products.ProductValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using MediatR;

namespace eCommerce.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;
        private readonly IOptionRepository _options;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateProductHandler(IProductRepository products,
            ICategoryRepository categories, IOptionRepository options,
            IUnitOfWork unitOfWork)
        {
            _products = products;
            _categories = categories;
            _options = options;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken ct)
        {
            var product = await _products.GetByIdAsync(request.Id, ct);
            if (product == null) throw new NotFoundException(
                    "Product for update not found from provided ID");

            var categories = await _categories.GetByIdsAsync(request.CategoryIds, ct);
            if (categories.Count != request.CategoryIds.Distinct().Count()) throw new NotFoundException(
                    "One or more categories were not found.");

            var options = await _options.GetByIdsAsync(request.OptionIds, ct);
            if (options.Count != request.OptionIds.Distinct().Count()) throw new NotFoundException(
                    "One or more options were not found.");

            var optionValuesById = options.SelectMany(o => o.Values)
                .ToDictionary(v => v.Id);

            product.Rename(new ProductName(request.Name));
            product.SetDescription(new ProductDescription(request.Description));

            product.ReplaceCategories(categories);
            product.ReplaceOptions(options);

            product.RemoveVariantsNotIn(
                request.ProductVariants
                    .Where(v => v.Id.HasValue)
                    .Select(v => v.Id!.Value));

            foreach (var variantRequest in request.ProductVariants)
            {
                var optionValues = variantRequest.OptionValueIds.Select(optionValueId =>
                {
                    if (!optionValuesById.TryGetValue(optionValueId, out var optionValue)) throw new NotFoundException(
                            $"Option value with ID {optionValueId} was not found among the selected product options.");

                    return optionValue;
                }).ToList();

                if (variantRequest.Id.HasValue)
                {
                    product.UpdateVariant(variantRequest.Id.Value, new ProductVariantSku(variantRequest.Sku),
                        new Money(variantRequest.Price), new StockQuantity(variantRequest.StockQuantity), optionValues);
                }
                else
                {
                    product.AddVariant(new ProductVariantSku(variantRequest.Sku), new Money(variantRequest.Price),
                        new StockQuantity(variantRequest.StockQuantity), optionValues);
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
