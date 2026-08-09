using eCommerce.Application.Features.Options.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Options.Commands.CreateOption
{
    public record CreateOptionCommand(
        string Name,
        List<string> OptionValues
    ) : IRequest<ReadOptionDto>;
}
