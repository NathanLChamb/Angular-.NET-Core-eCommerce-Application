using eCommerce.Application.Features.Auth.DTOs;
using eCommerce.Application.Interfaces;
using MediatR;

namespace eCommerce.Application.Features.Auth.Commands.Login;

public class LoginHandler(
    IIdentityService identityService,
    ITokenService tokenService,
    IAnonymousCartMergeService cartMergeService,
    IAnonymousCartSession anonymousSession)
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IAnonymousCartMergeService _cartMergeService = cartMergeService;
    private readonly IAnonymousCartSession _anonymousSession = anonymousSession;

    public async Task<AuthResponseDto> Handle(
        LoginCommand request,
        CancellationToken ct)
    {
        var user = await _identityService.LoginAsync(
            request.Email,
            request.Password)
            ?? throw new Exception("Invalid credentials");

        var anonymousId = _anonymousSession.GetAnonymousId();

        var token = _tokenService.CreateToken(user);

        await _cartMergeService.MergeAsync(
            user.Id,
            anonymousId,
            ct);

        if (anonymousId.HasValue)
            _anonymousSession.Clear();

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            DisplayName = user.DisplayName
        };
    }
}
