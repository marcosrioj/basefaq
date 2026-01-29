using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BaseFaq.Identity.Persistence.IdentityDb;
using BaseFaq.Identity.Persistence.IdentityDb.Extensions;
using BaseFaq.Identity.Persistence.IdentityDb.Models;

namespace BaseFaq.Identity.App.Extensions;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddIdentityDb(builder.Configuration.GetConnectionString("DefaultConnection"));


        builder.Services.AddIdentityServer(options => { })
            .AddAspNetIdentity<ApplicationUser>()
            .AddInMemoryIdentityResources([
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            ])
            .AddInMemoryApiScopes([
                new ApiScope("api1", "My API Web")
            ])
            .AddInMemoryClients([
                new Client
                {
                    ClientId = "react-spa",
                    ClientName = "React SPA Application",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RedirectUris = { "http://localhost:3000/callback" },
                    PostLogoutRedirectUris = { "http://localhost:3000/" },
                    AllowedCorsOrigins = { "http://localhost:3000" },
                    AllowedScopes = { "openid", "profile", "api1" }
                }
            ])
            .AddDeveloperSigningCredential();

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5001";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
                // options.Audience = "api1";
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "api1");
            });
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ReactCORSPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddRazorPages();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseCors("ReactCORSPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}