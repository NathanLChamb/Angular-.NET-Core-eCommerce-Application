using eCommerce.Application.Features.Options.DTOs;
using eCommerce.Application.Shared;
using MediatR;

namespace eCommerce.Application.Features.Options.Queries.GetAllOptions
{
    public record GetAllOptionsQuery(
        PaginationParams pageParams
    ) : IRequest<PagedResult<ReadOptionDto>>;
}
