using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Features.Options.DTOs;
using eCommerce.Application.Features.Products.DTOs;
using eCommerce.Application.Features.Products.Filters;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Shared;
using eCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Queries
{
    public class ProductQueries(
        eCommerceContext context) 
        : IProductQueries
    {
        private readonly eCommerceContext _context = context;

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
                    p.Name.Value.Contains(
                        filter.Search,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (filter.CategoryIds.Count > 0)
            {
                query = query.Where(p =>
                    p.Categories.Any(c =>
                        filter.CategoryIds.Contains(c.Id)));
            }

            if (filter.OptionIds.Count > 0)
            {
                query = query.Where(p =>
                    filter.OptionIds.All(optionId =>
                        p.Options.Any(o => o.Id == optionId)));
            }

            if (filter.OptionValueIds.Count > 0)
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
                    Options = p.Options.Select(o => new OptionResponse
                    {
                        Id = o.Id,
                        Name = o.Name,
                        OptionValues = o.Values.Select(ov => new OptionValueResponse
                        {
                            Id = ov.Id,
                            Value = ov.Value
                        }).ToList()
                    }).ToList(),
                    ProductVariants = p.Variants.Select(pv => new ProductVariantResponse
                    {
                        Id = pv.Id,
                        Sku = pv.Sku.Value,
                        Price = pv.Price.Amount,
                        StockQuantity = pv.StockQuantity.Value,
                        OptionValues = pv.OptionValues.Select(ov => new OptionValueResponse
                        {
                            Id = ov.Id,
                            Value = ov.Value
                        }).ToList(),
                        Pictures = pv.Pictures.Select(pc => new PictureResponse
                        {
                            Role = pc.Role,
                            Location = pc.Location.ToString()
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync(ct);

            return new PagedResult<ProductResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ProductResponse?> GetByIdAsync(
            int id, 
            CancellationToken ct)
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
                    Options = p.Options.Select(o => new OptionResponse
                    {
                        Id = o.Id,
                        Name = o.Name,
                        OptionValues = o.Values.Select(ov => new OptionValueResponse
                        {
                            Id = ov.Id,
                            Value = ov.Value
                        }).ToList()
                    }).ToList(),
                    ProductVariants = p.Variants.Select(pv => new ProductVariantResponse
                    {
                        Id = pv.Id,
                        Sku = pv.Sku.Value,
                        Price = pv.Price.Amount,
                        StockQuantity = pv.StockQuantity.Value,
                        OptionValues = pv.OptionValues.Select(ov => new OptionValueResponse
                        {
                            Id = ov.Id,
                            Value = ov.Value
                        }).ToList(),
                        Pictures = pv.Pictures.Select(pc => new PictureResponse
                        {
                            Role = pc.Role,
                            Location = pc.Location.ToString()
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<VariantDetailsResponse?> GetVariantDetailsAsync(
            int productVariantId, 
            CancellationToken ct)
        {
            return await _context.Products
                .SelectMany(p => p.Variants.Where(v => v.Id == productVariantId),
                    (p, v) => new VariantDetailsResponse
                    {
                        ProductVariantId = v.Id,
                        ProductName = p.Name.Value,
                        Sku = v.Sku.Value,
                        ImageUrl = v.Pictures.Select(pic => pic.Location.ToString()).FirstOrDefault(),
                        UnitPrice = v.Price.Amount
                    })
                    .SingleOrDefaultAsync(ct);
        }
    }
}

