using eCommerce.Application.Features.Products.DTOs;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Shared;
using MediatR;

namespace eCommerce.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, PagedResult<ProductResponse>>
    {
        private readonly IProductQueries _queries;
        public GetAllProductsHandler(IProductQueries queries)
        {
            _queries = queries;
        }

        public async Task<PagedResult<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken ct)
        {
            return await _queries.GetPagedAsync(request.Filter, ct);
        }
    }
}
