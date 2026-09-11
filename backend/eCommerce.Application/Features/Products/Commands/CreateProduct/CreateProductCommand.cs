using eCommerce.Application.Features.Products.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
        string Name,
        string Description,
        List<int> CategoryIds,
        List<int> OptionIds,
        List<CreateProductVariantRequest> ProductVariants
    ) : IRequest<ProductResponse>;
}
