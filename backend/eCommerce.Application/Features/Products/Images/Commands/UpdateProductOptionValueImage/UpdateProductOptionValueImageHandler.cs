using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Products.Images.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Domain.Catalog.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Features.Products.Images.Commands.UpdateProductOptionValueImage
{
    public class UpdateProductOptionValueImageHandler : IRequestHandler<UpdateProductOptionValueImageCommand, ProductOptionValueImageDto>
    {
        private readonly IeCommerceContext _context;
        public UpdateProductOptionValueImageHandler(IeCommerceContext context)
        {
            _context = context;
        }
        public async Task<ProductOptionValueImageDto> Handle(UpdateProductOptionValueImageCommand request, CancellationToken ct)
        {
            var image = await _context.ProductOptionValueImages
               .Include(x => x.OptionValues)
               .FirstOrDefaultAsync(x => x.Id == request.ImageId && x.ProductId == request.ProductId, ct);
            if (image == null) throw new NotFoundException("Product option value image not found.");
            
            var optionValueIds = request.OptionValueIds
                .Distinct()
                .ToList();

            if (optionValueIds.Count == 0) throw new ValidationRuleException("At least one option value is required.");

            var validOptionValueIds = await _context.Products
                .Where(p => p.Id == request.ProductId)
                    .SelectMany(p => p.Options)
                    .SelectMany(o => o.Values)
                .Where(ov => optionValueIds.Contains(ov.Id))
                .Select(ov => ov.Id)
                .ToListAsync(ct);
            if (validOptionValueIds.Count != optionValueIds.Count) throw new NotFoundException("One or more option values do not belong to this product.");
            
            image.Url = request.Url;

            image.OptionValues.Clear();

            foreach (var optionValueId in optionValueIds)
            {
                image.OptionValues.Add(new ProductOptionValueImageOptionValue
                    {
                        ProductOptionValueImageId = image.Id,
                        OptionValueId = optionValueId
                    });
            }

            if (request.IsDefault)
            {
                var existingDefaults = await _context.ProductOptionValueImages
                    .Where(x => x.ProductId == request.ProductId && x.Id != request.ImageId && x.IsDefault)
                    .ToListAsync(ct);

                foreach (var existingDefault in existingDefaults)
                {
                    existingDefault.IsDefault = false;
                }
            }
            image.IsDefault = request.IsDefault;

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
