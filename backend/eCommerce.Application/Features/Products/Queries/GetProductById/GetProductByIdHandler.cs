using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Products.DTOs;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using MediatR;

namespace eCommerce.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
    {
        private readonly IProductRepository _products;
        private readonly IProductQueries _queries;
        public GetProductByIdHandler(IProductRepository products, IProductQueries queries)
        {
            _products = products;
            _queries = queries;
        }

        public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken ct)
        {
            var product = await _products.GetByIdAsync(request.Id, ct);
            if (product == null) throw new NotFoundException("Product not found from provided ID");


            return await _queries.GetByIdAsync(product.Id, ct) ?? throw new NotFoundException(
                "Product could not be retrieved after saving");
        }
    }
}
