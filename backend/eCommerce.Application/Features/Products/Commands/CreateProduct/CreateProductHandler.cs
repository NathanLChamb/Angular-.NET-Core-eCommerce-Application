using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Products.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Products;
using eCommerce.Domain.Catalog.Products.ProductValueObjects;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ValueObjects;
using MediatR;

namespace eCommerce.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductHandler(
        IProductRepository products,
        ICategoryRepository categories, 
        IOptionRepository options,
        IProductQueries queries, 
        IUnitOfWork unitOfWork) 
        : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _products = products;
        private readonly ICategoryRepository _categories = categories;
        private readonly IOptionRepository _options = options;
        private readonly IProductQueries _queries = queries;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken ct)
        {
            var product = new Product(new Name(request.Name));

            product.SetDescription(new Description(request.Description));

            var categoryIds = request.CategoryIds.Distinct().ToList();
            var categories = await _categories.GetByIdsAsync(categoryIds, ct);
            if (categories.Count != request.CategoryIds.Distinct().Count()) throw new NotFoundException(
                "One or more categories were not found.");

            var optionIds = request.OptionIds.Distinct().ToList();
            var options = await _options.GetByIdsAsync(optionIds, ct);
            if (options.Count != request.OptionIds.Distinct().Count()) throw new NotFoundException(
                "One or more options were not found.");

            product.AddCategories(categories);
            product.AddOptions(options);

            var optionValuesById = options.SelectMany(o => o.Values)
                .ToDictionary(v => v.Id);

            var variants = request.ProductVariants.Select(variantRequest =>
            {
                var optionValues = variantRequest.OptionValueIds
                    .Select(optionValueId =>
                    {
                        if (!optionValuesById.TryGetValue(optionValueId, out var optionValue)) throw new NotFoundException(
                            $"Option value with ID {optionValueId} was not found among the selected product options.");

                        return optionValue;
                    })
                    .ToList();

                var pictures = variantRequest.Pictures
                    .Select(picture =>
                        new Picture(
                            picture.Role,
                            picture.Location)
                        )
                    .ToList();

                return new ProductVariant(new Sku(variantRequest.Sku), new Price(variantRequest.Price),
                    new StockQuantity(variantRequest.StockQuantity), optionValues, pictures);
            })
            .ToList();

            product.AddVariants(variants);

            await _products.AddAsync(product, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await _queries.GetByIdAsync(product.Id, ct) ?? throw new NotFoundException(
                "Product could not be retrieved after saving");
        }
    }
}
