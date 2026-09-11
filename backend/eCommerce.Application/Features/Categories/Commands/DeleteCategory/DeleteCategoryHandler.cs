using eCommerce.Application.Exceptions;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Repositories;
using MediatR;

namespace eCommerce.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoryRepository _categories;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCategoryHandler(ICategoryRepository categories, IUnitOfWork unitOfWork)
        {
            _categories = categories;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
        {
            var category = await _categories.GetByIdAsync(request.Id, ct);
            if (category == null) throw new NotFoundException("Category for deletion not found from provided ID");

            _categories.Remove(category);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
