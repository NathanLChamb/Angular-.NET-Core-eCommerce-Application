using eCommerce.Application.Features.Categories.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(
        string Name
        ) : IRequest<CategoryResponse>;
}
