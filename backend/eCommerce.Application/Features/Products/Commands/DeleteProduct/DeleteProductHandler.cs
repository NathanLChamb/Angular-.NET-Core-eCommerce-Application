using eCommerce.Application.Exceptions;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Products;
using MediatR;

namespace eCommerce.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _products;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteProductHandler(IProductRepository products, IUnitOfWork unitOfWork)
        {
            _products = products;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken ct)
        {
            var product = await _products.GetByIdAsync(request.Id, ct);
            if (product == null) throw new NotFoundException("Product for deletion not found from provided ID");

            _products.Remove(product);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
