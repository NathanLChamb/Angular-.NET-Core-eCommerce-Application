using eCommerce.Api.Middleware;
using eCommerce.Application;
using eCommerce.Application.Interfaces;
using eCommerce.Infrastructure.Identity;
using eCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!;
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddScoped<IeCommerceContext>(provider =>
    provider.GetRequiredService<eCommerceContext>());


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<eCommerceContext>();

    db.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();

    await seeder.SeedAsync();
}

app.UseCors();

app.UseMiddleware<GlobalExceptionHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
