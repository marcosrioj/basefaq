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
                Version = version
            });
        });
    }

    public static void AddSwaggerWithAuth(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.LoadSwaggerOptions(configuration);

        var options = configuration.GetSection("SwaggerOptions").Get<SwaggerOptions>();
        var scopeList = configuration
            .GetSection("SwaggerOptions:swaggerAuth:Scopes")
            .GetChildren()
            .Select(child => child.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .ToArray();
        var effectiveScopes = scopeList.ToDictionary(scope => scope, _ => "API access");

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
                Version = version
            });

            var scheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,

                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(options!.swaggerAuth!.AuthorizeEndpoint),
                        TokenUrl = new Uri(options.swaggerAuth.TokenEndpoint),
                        Scopes = effectiveScopes
                    }
                },
                Description = "Server OpenId Security Scheme"
            };


            if (options.swaggerAuth.EnableClientCredentials)
            {
                scheme.Flows.ClientCredentials = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(options.swaggerAuth.AuthorizeEndpoint),
                    TokenUrl = new Uri(options.swaggerAuth.TokenEndpoint),
                    Scopes = effectiveScopes
                };
            }

            c.AddSecurityDefinition("OAuth2", scheme);

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("OAuth2", document)] = []
            });
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

            var scopeKeys = configuration
                .GetSection("SwaggerOptions:swaggerAuth:ScopeList")
                .GetChildren()
                .Select(child => child.Value)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToArray();

            if (scopeKeys is { Length: > 0 })
            {
                options.OAuthScopes(scopeKeys);
            }

            options.OAuthUsePkce();
            options.OAuthScopeSeparator(" ");
            options.EnablePersistAuthorization();
        });

        return app;
    }
}