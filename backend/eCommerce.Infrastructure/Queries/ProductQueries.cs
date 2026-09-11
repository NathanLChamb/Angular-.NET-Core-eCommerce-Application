using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Features.Products.DTOs;
using eCommerce.Application.Features.Products.Filters;
using eCommerce.Application.Features.Products.Images.DTOs;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Shared;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Persistence.Queries
{
    public class ProductQueries : IProductQueries
    {
        private readonly eCommerceContext _context;
        public ProductQueries(eCommerceContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductResponse>> GetPagedAsync(
            ProductSearchFilter filter,
            CancellationToken ct)
        {
            var query = _context.Products
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(p =>
                    p.Name.Value.ToLower().Contains(filter.Search.ToLower()));
            }

            if (filter.CategoryIds.Any())
            {
                query = query.Where(p =>
                    p.Categories.Any(c =>
                        filter.CategoryIds.Contains(c.Id)));
            }

            if (filter.OptionIds.Any())
            {
                query = query.Where(p =>
                    filter.OptionIds.All(optionId =>
                        p.Options.Any(o => o.Id == optionId)));
            }

            if (filter.OptionValueIds.Any())
            {
                query = query.Where(p =>
                    p.Variants.Any(v =>
                        filter.OptionValueIds.All(valueId =>
                            v.OptionValues.Any(ov => ov.Id == valueId))));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Variants.Any(v =>
                        v.Price!.Amount >= filter.MinPrice.Value));
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Variants.Any(v =>
                        v.Price!.Amount <= filter.MaxPrice.Value));
            }

            query = filter.Sort switch
            {
                ProductSort.PriceAscending => query.OrderBy(
                    p => p.Variants.Min(v => v.Price.Amount)),

                ProductSort.PriceDescending => query.OrderByDescending(
                    p => p.Variants.Max(v => v.Price.Amount)),

                _ => query.OrderBy(p => p.Id)
            };

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name.Value,
                    Description = p.Description.Value,
                    Categories = p.Categories.Select(c => new CategoryResponse
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList(),
                    Options = p.Options.Select(o => new ProductOptionResponse
                    {
                        Id = o.Id,
                        Name = o.Name
                    }).ToList(),
                    ProductVariants = p.Variants.Select(pv => new ProductVariantResponse
                    {
                        Id = pv.Id,
                        Sku = pv.Sku.Value,
                        Price = pv.Price!.Amount,
                        StockQuantity = pv.StockQuantity.Value,

                        OptionValues = pv.OptionValues
                            .Select(ov =>
                                new ProductVariantOptionValueResponse
                                {
                                    Id = ov.Id,
                                    Value = ov.Value,
                                    OptionId = EF.Property<int>(
                                        ov,
                                        "OptionId")
                                })
                            .ToList()
                    }).ToList(),
                    ProductImages = p.ProductImages.Select(pi => new ProductImageDto
                    {
                        Id = pi.Id,
                        Url = pi.Url,
                        DisplayOrder = pi.DisplayOrder
                    }).ToList(),
                    ProductOptionValueImages = p.ProductOptionValueImages.Select(image => new ProductOptionValueImageDto
                    {
                        Id = image.Id,
                        Url = image.Url,
                        DisplayOrder = image.DisplayOrder,
                        IsDefault = image.IsDefault,

                        OptionValueIds = image.OptionValues
                            .Select(x => x.OptionValueId)
                            .ToList()
                    }).ToList()
                }).ToListAsync(ct);

            return new PagedResult<ProductResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name.Value,
                    Description = p.Description.Value,
                    Categories = p.Categories.Select(c => new CategoryResponse
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList(),
                    Options = p.Options.Select(o => new ProductOptionResponse
                    {
                        Id = o.Id,
                        Name = o.Name
                    }).ToList(),
                    ProductVariants = p.Variants.Select(pv => new ProductVariantResponse
                    {
                        Id = pv.Id,
                        Sku = pv.Sku.Value,
                        Price = pv.Price!.Amount,
                        StockQuantity = pv.StockQuantity.Value,
                        OptionValues = pv.OptionValues.Select(ov => new ProductVariantOptionValueResponse
                        {
                            Id = ov.Id,
                            Value = ov.Value,
                            OptionId = EF.Property<int>(
                                ov,
                                "OptionId")
                        }).ToList()
                    }).ToList(),
                    ProductImages = p.ProductImages.Select(pi => new ProductImageDto
                    {
                        Id = pi.Id,
                        Url = pi.Url,
                        DisplayOrder = pi.DisplayOrder
                    }).ToList(),
                    ProductOptionValueImages = p.ProductOptionValueImages.Select(image => new ProductOptionValueImageDto
                    {
                        Id = image.Id,
                        Url = image.Url,
                        DisplayOrder = image.DisplayOrder,
                        IsDefault = image.IsDefault,

                        OptionValueIds = image.OptionValues
                            .Select(x => x.OptionValueId)
                            .ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}

