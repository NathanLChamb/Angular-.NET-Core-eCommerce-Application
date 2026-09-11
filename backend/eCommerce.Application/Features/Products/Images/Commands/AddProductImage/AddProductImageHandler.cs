using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Products.Images.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Domain.Catalog.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Features.Products.Images.Commands.AddProductImage
{
    public class AddProductImageHandler
        : IRequestHandler<AddProductImageCommand, ProductImageDto>
    {
        private readonly IeCommerceContext _context;

        public AddProductImageHandler(IeCommerceContext context)
        {
            _context = context;
        }


        public async Task<ProductImageDto> Handle(
            AddProductImageCommand request,
            CancellationToken ct)
        {
            var productExists =
                await _context.Products
                    .AnyAsync(
                        p => p.Id == request.ProductId,
                        ct);

            if (!productExists)
                throw new NotFoundException("Product not found.");


            var image = new ProductImage
            {
                ProductId = request.ProductId,
                Url = request.Url,
                DisplayOrder = request.DisplayOrder
            };


            _context.ProductImages.Add(image);

            await _context.SaveChangesAsync(ct);


            return new ProductImageDto
            {
                Id = image.Id,
                Url = image.Url,
                DisplayOrder = image.DisplayOrder
            };
        }
    }
}