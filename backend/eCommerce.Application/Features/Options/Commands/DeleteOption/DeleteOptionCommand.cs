using MediatR;

namespace eCommerce.Application.Features.Options.Commands.DeleteOption
{
    public record DeleteOptionCommand(
        int Id
    ) : IRequest;
}
