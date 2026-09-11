using eCommerce.Application.Exceptions;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using MediatR;

namespace eCommerce.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categories;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCategoryHandler(ICategoryRepository categories, IUnitOfWork unitOfWork)
        {
            _categories = categories;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken ct)
        {
            var category = await _categories.GetByIdAsync(request.Id, ct);
            if (category == null) throw new NotFoundException(
                "Category for update not found from provided ID");

            category.Rename(request.Name);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
