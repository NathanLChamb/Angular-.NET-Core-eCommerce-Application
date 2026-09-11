using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Products.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Products;
using eCommerce.Domain.Catalog.Products.ProductValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using MediatR;

namespace eCommerce.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;
        private readonly IOptionRepository _options;
        private readonly IProductQueries _queries;
        private readonly IUnitOfWork _unitOfWork;
        public CreateProductHandler(IProductRepository products,
            ICategoryRepository categories, IOptionRepository options,
            IProductQueries queries, IUnitOfWork unitOfWork)
        {
            _products = products;
            _categories = categories;
            _options = options;
            _queries = queries;
            _unitOfWork = unitOfWork;
        }
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken ct)
        {
            var product = new Product(new ProductName(request.Name));

            product.SetDescription(new ProductDescription(request.Description));

            var categories = await _categories.GetByIdsAsync(request.CategoryIds, ct);

            if (categories.Count != request.CategoryIds.Distinct().Count()) throw new NotFoundException(
                "One or more categories were not found.");

            foreach (var category in categories)
            {
                product.AddCategory(category);
            }

            var options = await _options.GetByIdsAsync(request.OptionIds, ct);

            if (options.Count != request.OptionIds.Distinct().Count()) throw new NotFoundException(
                "One or more options were not found.");

            foreach (var option in options)
            {
                product.AddOption(option);
            }

            var optionValuesById = options.SelectMany(o => o.Values)
                    .ToDictionary(v => v.Id);

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

                product.AddVariant(new ProductVariantSku(variantRequest.Sku), new Money(variantRequest.Price),
                    new StockQuantity(variantRequest.StockQuantity), optionValues);
            }

            await _products.AddAsync(product, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await _queries.GetByIdAsync(product.Id, ct) ?? throw new NotFoundException(
                "Product could not be retrieved after saving");
        }
    }
}
