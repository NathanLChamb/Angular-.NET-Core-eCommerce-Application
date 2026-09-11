using eCommerce.Application.Features.Options.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Application.Mapping;
using eCommerce.Application.Shared;
using MediatR;

namespace eCommerce.Application.Features.Options.Queries.GetAllOptions
{
    public class GetAllOptionsHandler : IRequestHandler<GetAllOptionsQuery, PagedResult<OptionResponse>>
    {
        private readonly IOptionQueries _queries;
        public GetAllOptionsHandler(IOptionQueries queries)
        {
            _queries = queries;
        }

        public async Task<PagedResult<OptionResponse>> Handle(GetAllOptionsQuery request, CancellationToken ct)
        {
            return await _queries.GetPagedAsync(request.pageParams, ct);
        }
    }
}
