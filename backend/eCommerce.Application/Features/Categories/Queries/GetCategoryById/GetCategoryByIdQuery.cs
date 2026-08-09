using eCommerce.Application.Features.Categories.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Categories.Queries.GetCategoryById
{
    public record GetCategoryByIdQuery(
        int Id
    ) : IRequest<ReadCategoryDto>;
}
