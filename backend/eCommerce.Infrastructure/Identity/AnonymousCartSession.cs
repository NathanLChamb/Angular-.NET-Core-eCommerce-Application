using eCommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace eCommerce.Infrastructure.Identity
{
    public class AnonymousCartSession(
        IHttpContextAccessor accessor) 
        : IAnonymousCartSession
    {
        private const string CookieName = "anonymous_id";
        public Guid? GetAnonymousId()
        {
            var context = accessor.HttpContext;

            if (context is null)
                return null;

            if (!context.Request.Cookies.TryGetValue(
                    CookieName, out var value))
            {
                return null;
            }

            return Guid.TryParse(value, out var id) ? id : null;
        }

        public void Clear()
        {
            accessor.HttpContext?.Response.Cookies.Delete(
                CookieName,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/"
                });
        }
    }
}
