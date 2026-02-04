using BaseFaq.Common.Infrastructure.Core.Extensions;
using BaseFaq.Common.Infrastructure.MediatR.Extensions;
using BaseFaq.Common.Infrastructure.Mvc.Filters;
using BaseFaq.Common.Infrastructure.Sentry.Extensions;
using BaseFaq.Common.Infrastructure.Swagger.Extensions;
using BaseFaq.Common.EntityFramework.Tenant.Extensions;
using BaseFaq.Faq.FaqWeb.App.Extensions;
using BaseFaq.Faq.Infrastructure.ApiErrorHandling.Extensions;
using BaseFaq.Faq.FaqWeb.Business.Faq.Authorization;
using BaseFaq.Models.Tenant.Enums;

namespace BaseFaq.Faq.FaqWeb.App;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseDefaultServiceProvider(opt =>
        {
            opt.ValidateOnBuild = true;
            opt.ValidateScopes = true;
        });

        // Add services to the container.
        builder.Services.AddTenantRoleAuthorization(options =>
        {
            options.AddTenantRolePolicy(FaqAuthorizationPolicies.Get, UserRoleType.Member, UserRoleType.Admin);
            options.AddTenantRolePolicy(FaqAuthorizationPolicies.GetList, UserRoleType.Member, UserRoleType.Admin);
            options.AddTenantRolePolicy(FaqAuthorizationPolicies.Create, UserRoleType.Admin);
            options.AddTenantRolePolicy(FaqAuthorizationPolicies.Update, UserRoleType.Admin);

            options.AddTenantRolePolicy(FaqItemAuthorizationPolicies.Get, UserRoleType.Member, UserRoleType.Admin);
            options.AddTenantRolePolicy(FaqItemAuthorizationPolicies.GetList, UserRoleType.Member, UserRoleType.Admin);
            options.AddTenantRolePolicy(FaqItemAuthorizationPolicies.Create, UserRoleType.Admin);
            options.AddTenantRolePolicy(FaqItemAuthorizationPolicies.Update, UserRoleType.Admin);
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.LoadCustomCorsOptions(builder.Configuration);

        builder.Services.LoadJwtAuthenticationOptions(builder.Configuration);
        builder.Services.LoadSessionOptions(builder.Configuration);

        builder.Services.AddCustomForwardedHeaders();

        builder.Services.AddCustomCors(builder.Configuration);

        builder.Services.AddSwaggerWithAuth(builder.Configuration);

        builder.Services.AddIdentityService(builder.Configuration);

        builder.Services.AddDefaultAuthentication(builder.Configuration);

        builder.Services.AddTenantDb(builder.Configuration.GetConnectionString("TenantDb"));

        builder.Services.AddLogging(c =>
        {
            c.SetMinimumLevel(LogLevel.Information);
            c.AddConsole();
        });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddFeatures(builder.Configuration);

        builder.Services.AddMediatRLogging();

        builder.WebHost.AddConfiguredSentry();

        builder.Services.AddControllers(options => { options.Filters.Add(new StringTrimmingActionFilter()); })
            .AddJsonOptions(options => { });

        var app = builder.Build();

        app.UseCustomForwardedHeaders();

        app.UseApiErrorHandlingMiddleware();

        app.UseRouting();

        if (!app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.OAuthClientId("basefaq-api-swagger-dev");
                options.OAuthClientSecret("secret");
                options.OAuthScopes("profile", "openid", "BaseFaq.App");
                options.EnablePersistAuthorization();
            });

            app.MapOpenApi();
        }

        app.UseCustomCors(builder.Configuration);

        app.UseHttpsRedirection();

        app.UseConfiguredSentry();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers().RequireAuthorization();

        app.Run();
    }
}