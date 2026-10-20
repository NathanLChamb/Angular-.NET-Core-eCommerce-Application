using eCommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace eCommerce.Infrastructure.Identity
{
    public class CurrentUser(
        IHttpContextAccessor httpContextAccessor) 
        : ICurrentUser
    {
        private const string AnonymousIdCookie = "anonymous_id";
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private Guid? _anonymousId;

        public string? UserId
        {

            get
            {
                var context = _httpContextAccessor.HttpContext;

                if (context?.User.Identity?.IsAuthenticated != true)
                    return null;

                return context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? context.User.FindFirstValue("sub");
            }
        }
        

        public Guid? AnonymousId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(UserId))
                    return null;

                if (_anonymousId.HasValue)
                    return _anonymousId;

                var context = _httpContextAccessor.HttpContext;

                if (context == null)
                    return null;

                if (context.Request.Cookies.TryGetValue(AnonymousIdCookie, out var value) &&
                    Guid.TryParse(value, out var anonymousId))
                {
                    _anonymousId = anonymousId;
                    return _anonymousId;
                }

                _anonymousId = Guid.NewGuid();

                context.Response.Cookies.Append(
                    AnonymousIdCookie,
                    _anonymousId.Value.ToString(),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddYears(1)
                    });

                return _anonymousId;
            }
        }
    }
}
