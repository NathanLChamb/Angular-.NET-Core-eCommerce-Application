using eCommerce.Application.Exceptions;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Repositories;
using MediatR;

namespace eCommerce.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductHandler(
        IProductRepository products, 
        IUnitOfWork unitOfWork) 
        : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _products = products;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(DeleteProductCommand request, CancellationToken ct)
        {
            var product = await _products.GetByIdAsync(request.Id, ct)
                ?? throw new NotFoundException(
                    "Product for deletion not found from provided ID");

            _products.Remove(product);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
