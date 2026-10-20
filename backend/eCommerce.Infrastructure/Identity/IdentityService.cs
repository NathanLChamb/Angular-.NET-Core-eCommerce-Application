using eCommerce.Application.Common.Constants;
using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace eCommerce.Infrastructure.Identity
{
    public class IdentityService(
        UserManager<ApplicationUser> userManager)
        : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<TokenUser?> LoginAsync(string email, string password)
        {
            email = email.Trim().ToLowerInvariant();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var validPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!validPassword) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new TokenUser
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Roles = roles
            };
        }

        public async Task<TokenUser> RegisterAsync(string email, string password, string displayName)
        {
            email = email.Trim().ToLowerInvariant();

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new BusinessRuleException("Email is already registered.");
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                DisplayName = displayName
            };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new BusinessRuleException(errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new BusinessRuleException($"Failed assigning customer role: {errors}");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new TokenUser
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Roles = roles
            };
        }
    }
}