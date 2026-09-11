using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Domain.Catalog.Categories;

namespace eCommerce.Application.Features.Categories.Mappings
{
    public static class CategoryProjection
    {
        public static CategoryResponse MapToResponse(this Category category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
