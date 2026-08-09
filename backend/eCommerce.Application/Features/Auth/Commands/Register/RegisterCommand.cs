using eCommerce.Application.Features.Auth.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(
        string Email,
        string Password,
        string DisplayName
    ) : IRequest<AuthResponseDto>;
}
