using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Shared;
using MediatR;

namespace eCommerce.Application.Features.Categories.Queries.GetAllCategories
{
    public record GetAllCategoriesQuery(
        PaginationParams pageParams
        ) : IRequest<PagedResult<CategoryResponse>>;
}
