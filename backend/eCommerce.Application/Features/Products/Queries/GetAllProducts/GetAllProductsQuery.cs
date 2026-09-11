using eCommerce.Application.Features.Products.DTOs;
using eCommerce.Application.Features.Products.Filters;
using eCommerce.Application.Shared;
using MediatR;

namespace eCommerce.Application.Features.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(
        ProductSearchFilter Filter
        ) : IRequest<PagedResult<ProductResponse>>;
}
