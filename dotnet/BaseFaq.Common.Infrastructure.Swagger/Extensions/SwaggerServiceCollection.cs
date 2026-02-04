using BaseFaq.Common.Infrastructure.Swagger.Options;
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
                        Scopes = options.swaggerAuth.Scopes
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
                    Scopes = options.swaggerAuth.Scopes
                };
            }

            c.AddSecurityDefinition("OAuth2", scheme);

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("OAuth2", document)] = []
            });
        });
    }
}