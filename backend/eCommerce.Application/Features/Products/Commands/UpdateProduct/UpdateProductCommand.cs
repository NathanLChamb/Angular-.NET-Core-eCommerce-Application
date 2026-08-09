using eCommerce.Application.Features.Products.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(
        int Id,
        string Name,
        string Description,
        List<int> CategoryIds,
        List<int> OptionIds,
        List<UpdateProductVariantDto> ProductVariants
    ) : IRequest;
}
