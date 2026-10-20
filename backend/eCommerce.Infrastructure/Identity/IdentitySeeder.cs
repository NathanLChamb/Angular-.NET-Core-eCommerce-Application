using eCommerce.Application.Common.Constants;
using Microsoft.AspNetCore.Identity;

namespace eCommerce.Infrastructure.Identity
{
    public class IdentitySeeder(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedCustomerUserAsync();
            await SeedAdminUserAsync();
        }
        private async Task SeedRolesAsync()
        {
            var roles = new[]
            {
            Roles.Admin,
            Roles.Customer
        };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Failed creating role {role}: {errors}");
                    }
                }
            }
        }
        private async Task SeedCustomerUserAsync()
        {
            const string userEmail = "test@example.com";
            const string userPassword = "Customer123!";

            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, Roles.Customer))
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);

                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        throw new Exception($"Failed assigning customer role: {errors}");
                    }
                }

                return;
            }

            user = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                DisplayName = "Test Customer"
            };

            var createResult = await _userManager.CreateAsync(user, userPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed creating customer user: {errors}");
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);

            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed assigning customer role: {errors}");
            }
        }
        private async Task SeedAdminUserAsync()
        {
            const string adminEmail = "admin@ecommerce.com";
            const string adminPassword = "Admin123!";

            var admin = await _userManager.FindByEmailAsync(adminEmail);

            if (admin != null)
            {
                if (!await _userManager.IsInRoleAsync(admin, Roles.Admin))
                {
                    await _userManager.AddToRoleAsync(admin, Roles.Admin);
                }
                return;
            }

            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Administrator"
            };

            var createResult = await _userManager.CreateAsync(admin, adminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed creating admin user: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(admin, Roles.Admin);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed assigning admin role: {errors}");
            }
        }
    }
}