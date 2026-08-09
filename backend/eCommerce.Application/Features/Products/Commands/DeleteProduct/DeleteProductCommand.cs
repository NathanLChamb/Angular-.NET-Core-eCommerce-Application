using MediatR;

namespace eCommerce.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(
        int Id
    ) : IRequest;
}
