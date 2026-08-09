using MediatR;

namespace eCommerce.Application.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(
        int Id,
        string Name,
        string Description
    ) : IRequest;
}
