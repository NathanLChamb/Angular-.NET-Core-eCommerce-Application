using eCommerce.Application.Features.Categories.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Categories.Queries.GetCategoryById
{
    public record GetCategoryToEditQuery(
        int Id
    ) : IRequest<CategoryResponse>;
}
