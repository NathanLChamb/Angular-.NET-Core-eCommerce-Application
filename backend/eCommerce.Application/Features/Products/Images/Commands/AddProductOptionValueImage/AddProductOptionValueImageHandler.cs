using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Products.Images.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Domain.Catalog.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Features.Products.Images.Commands.AddProductOptionValueImage
{
    public class AddProductOptionValueImageHandler : IRequestHandler<AddProductOptionValueImageCommand, ProductOptionValueImageDto>
    {
        private readonly IeCommerceContext _context;
        public AddProductOptionValueImageHandler(IeCommerceContext context)
        {
            _context = context;
        }
        public async Task<ProductOptionValueImageDto> Handle(AddProductOptionValueImageCommand request, CancellationToken ct)
        {
            var productExists = await _context.Products
                .AnyAsync(p => p.Id == request.ProductId, ct);

            if (!productExists) throw new NotFoundException("Product not found.");

            var optionValueIds = request.OptionValueIds
                .Distinct()
                .ToList();

            var validOptionValueIds = await _context.Products
                .Where(p => p.Id == request.ProductId)
                    .SelectMany(p => p.Options)
                    .SelectMany(o => o.Values)
                .Where(ov => optionValueIds.Contains(ov.Id))
                .Select(ov => ov.Id)
                .ToListAsync(ct);

            if (validOptionValueIds.Count != optionValueIds.Count) throw new NotFoundException("One or more option values do not belong to this product.");

            if (request.IsDefault)
            {
                var currentDefaults = await _context.ProductOptionValueImages
                    .Where(x => x.ProductId == request.ProductId && x.IsDefault)
                    .ToListAsync(ct);

                foreach (var existing in currentDefaults)
                {
                    existing.IsDefault = false;
                }
            }

            var existingImages = await _context.ProductOptionValueImages
                .Where(image =>
                    image.ProductId == request.ProductId &&
                    image.OptionValues.Count == optionValueIds.Count &&
                    image.OptionValues.All(x =>
                        optionValueIds.Contains(x.OptionValueId)))
                .ToListAsync(ct);

            var displayOrder = existingImages.Count;

            var image = new ProductOptionValueImage
            {
                ProductId = request.ProductId,
                Url = request.Url,
                DisplayOrder = displayOrder,
                IsDefault = request.IsDefault
            };

            foreach (var optionValueId in optionValueIds)
            {
                image.OptionValues.Add(new ProductOptionValueImageOptionValue
                {
                    OptionValueId = optionValueId
                });
            }

            _context.ProductOptionValueImages.Add(image);

            await _context.SaveChangesAsync(ct);

            return new ProductOptionValueImageDto
            {
                Id = image.Id,
                Url = image.Url,
                DisplayOrder = image.DisplayOrder,
                IsDefault = image.IsDefault,
                OptionValueIds = optionValueIds
            };
        }
    }
}