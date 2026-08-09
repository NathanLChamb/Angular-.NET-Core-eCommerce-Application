using MediatR;

namespace eCommerce.Application.Features.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(
        int Id
    ) : IRequest;
}
