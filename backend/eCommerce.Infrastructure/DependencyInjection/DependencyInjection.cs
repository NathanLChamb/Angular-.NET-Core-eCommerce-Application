using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Infrastructure.Identity;
using eCommerce.Infrastructure.Persistence;
using eCommerce.Infrastructure.Persistence.Queries;
using eCommerce.Infrastructure.Persistence.Repositories;
using eCommerce.Infrastructure.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<eCommerceContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnectionString")));

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredLength = 8;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<eCommerceContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),

                NameClaimType = JwtRegisteredClaimNames.Email,
                RoleClaimType = ClaimTypes.Role
            };
        });

        
        services.AddAuthorization();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IdentitySeeder>();

        services.AddScoped<ICartQueries, CartQueries>();
        services.AddScoped<IProductQueries, ProductQueries>();
        services.AddScoped<IOptionQueries, OptionQueries>();
        services.AddScoped<ICategoryQueries, CategoryQueries>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOptionRepository, OptionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<eCommerceContext>());

        return services;
    }
}