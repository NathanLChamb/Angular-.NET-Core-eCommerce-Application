using eCommerce.Application.Features.Options.DTOs;
using eCommerce.Application.Interfaces.Queries;
using MediatR;

namespace eCommerce.Application.Features.Options.Queries.GetOptionById
{
    public class GetOptionToEditHandler : IRequestHandler<GetOptionByIdQuery, OptionResponse>
    {
        private readonly IOptionQueries _queries;
        public GetOptionToEditHandler(IOptionQueries queries)
        {
            _queries = queries;
        }

        public async Task<OptionResponse> Handle(GetOptionByIdQuery request, CancellationToken ct)
        {
            return await _queries.GetByIdAsync(request.Id, ct);
        }
    }
}
