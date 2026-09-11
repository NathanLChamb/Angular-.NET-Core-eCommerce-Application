using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Features.Categories.Mappings;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using MediatR;

namespace eCommerce.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryToEditHandler : IRequestHandler<GetCategoryToEditQuery, CategoryResponse>
    {
        private readonly ICategoryRepository _categories;
        public GetCategoryToEditHandler(ICategoryRepository categories)
        {
            _categories = categories;
        }

        public async Task<CategoryResponse> Handle(GetCategoryToEditQuery request, CancellationToken ct)
        {
            var category = await _categories.GetByIdAsync(request.Id, ct);
            if (category == null) throw new NotFoundException("Category not found from provided ID");

            return CategoryProjection.MapToResponse(category);
        }
    }
}
