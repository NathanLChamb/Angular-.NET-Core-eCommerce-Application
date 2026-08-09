using eCommerce.Application.Features.Options.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Options.Commands.UpdateOption
{
    public record UpdateOptionCommand(
        int Id,
        string Name,
        List<UpdateOptionValueDto> OptionValues
    ) : IRequest;
}
