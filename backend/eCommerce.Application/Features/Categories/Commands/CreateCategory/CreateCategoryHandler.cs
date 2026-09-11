using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Features.Categories.Mappings;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Categories;
using MediatR;

namespace eCommerce.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
    {
        private readonly ICategoryRepository _categories;
        private readonly IUnitOfWork _unitOfWork;
        public CreateCategoryHandler(ICategoryRepository categories, ICategoryQueries queries, IUnitOfWork unitOfWork)
        {
            _categories = categories;
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken ct)
        {
            var category = new Category(request.Name);

            await _categories.AddAsync(category, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return CategoryProjection.MapToResponse(category);
        }
    }
}
