using BaseFaq.Common.Infrastructure.Core.Middleware;
using BaseFaq.Common.Infrastructure.Swagger.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace BaseFaq.Common.Infrastructure.Swagger.Extensions;

public static class SwaggerServiceCollection
{
    public static IServiceCollection LoadSwaggerOptions(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddOptions<SwaggerOptions>()
            .Bind(configuration.GetSection(SwaggerOptions.Name))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }


    public static void AddSwagger(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.LoadSwaggerOptions(configuration);

        var options = configuration.GetSection("SwaggerOptions").Get<SwaggerOptions>();

        services.AddSwaggerGen(c =>
        {
            c.UseOneOfForPolymorphism();
            c.EnableAnnotations();
            c.CustomSchemaIds(type => type.FullName);

            var version = options?.Version ?? "v1";
            var title = options?.Title ?? "Swagger API";

            c.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = "Tenant context is provided via X-Tenant-Id headers and resolved by middleware."
            });

            if (options?.EnableTenantHeader ?? true)
            {
                var tenantScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = TenantResolutionMiddleware.TenantHeaderName,
                    In = ParameterLocation.Header,
                    Description = "Set the tenant once in the Authorize dialog."
                };

                c.AddSecurityDefinition("TenantId", tenantScheme);
                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("TenantId", document)] = []
                });
            }
        });
    }

    public static void AddSwaggerWithAuth(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.LoadSwaggerOptions(configuration);

        var options = configuration.GetSection("SwaggerOptions").Get<SwaggerOptions>();

        services.AddSwaggerGen(c =>
        {
            c.UseOneOfForPolymorphism();
            c.EnableAnnotations();
            c.CustomSchemaIds(type => type.FullName);

            var version = options?.Version ?? "v1";
            var title = options?.Title ?? "Swagger API";

            c.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = "Tenant context is provided via X-Tenant-Id headers and resolved by middleware."
            });

            var scheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,

                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(options!.swaggerAuth!.AuthorizeEndpoint),
                        TokenUrl = new Uri(options.swaggerAuth.TokenEndpoint)
                    }
                },
                Description = "Server OpenId Security Scheme"
            };


            if (options.swaggerAuth.EnableClientCredentials)
            {
                scheme.Flows.ClientCredentials = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(options.swaggerAuth.AuthorizeEndpoint),
                    TokenUrl = new Uri(options.swaggerAuth.TokenEndpoint)
                };
            }

            c.AddSecurityDefinition("OAuth2", scheme);

            if (options?.EnableTenantHeader ?? true)
            {
                var tenantScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = TenantResolutionMiddleware.TenantHeaderName,
                    In = ParameterLocation.Header,
                    Description = "Set the tenant once in the Authorize dialog."
                };

                c.AddSecurityDefinition("TenantId", tenantScheme);
                c.AddSecurityRequirement(document =>
                {
                    var requirement = new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("OAuth2", document)] = []
                    };

                    requirement[new OpenApiSecuritySchemeReference("TenantId", document)] = [];
                    return requirement;
                });
            }
            else
            {
                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("OAuth2", document)] = []
                });
            }
        });
    }

    public static IApplicationBuilder UseSwaggerUIWithAuth(this IApplicationBuilder app)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

        app.UseSwaggerUI(options =>
        {
            var swaggerAuth = configuration.GetSection("SwaggerOptions")
                .Get<SwaggerOptions>()?.swaggerAuth;

            if (!string.IsNullOrWhiteSpace(swaggerAuth?.ClientId))
            {
                options.OAuthClientId(swaggerAuth.ClientId);
            }

            if (!string.IsNullOrWhiteSpace(swaggerAuth?.ClientSecret))
            {
                options.OAuthClientSecret(swaggerAuth.ClientSecret);
            }

            if (!string.IsNullOrWhiteSpace(swaggerAuth?.Audience))
            {
                options.OAuthAdditionalQueryStringParams(new Dictionary<string, string>
                {
                    ["audience"] = swaggerAuth.Audience
                });
            }

            options.EnablePersistAuthorization();
        });

        return app;
    }
}