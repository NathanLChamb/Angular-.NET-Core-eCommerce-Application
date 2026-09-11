using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Shared;
using MediatR;

namespace eCommerce.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, PagedResult<CategoryResponse>>
    {
        private readonly ICategoryQueries _queries;
        public GetAllCategoriesHandler(ICategoryQueries queries, IUnitOfWork unitOfWork)
        {
            _queries = queries;
        }

        public async Task<PagedResult<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken ct)
        {
            return await _queries.GetPagedAsync(request.pageParams, ct);
        }
    }
}
