using eCommerce.Application.Features.Options.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Options.Queries.GetOptionById
{
    public record GetOptionByIdQuery(
        int Id
    ) : IRequest<ReadOptionDto>;
}
