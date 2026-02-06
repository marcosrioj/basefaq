using BaseFaq.Common.Infrastructure.Core.Extensions;
using BaseFaq.Common.Infrastructure.MediatR.Extensions;
using BaseFaq.Common.Infrastructure.Mvc.Filters;
using BaseFaq.Common.Infrastructure.Sentry.Extensions;
using BaseFaq.Common.Infrastructure.Swagger.Extensions;
using BaseFaq.Common.EntityFramework.Tenant.Extensions;
using BaseFaq.Faq.FaqWeb.App.Extensions;
using BaseFaq.Faq.Infrastructure.ApiErrorHandling.Extensions;

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
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.LoadCustomCorsOptions(builder.Configuration);

        builder.Services.LoadJwtAuthenticationOptions(builder.Configuration);
        builder.Services.AddCustomForwardedHeaders();

        builder.Services.AddCustomCors(builder.Configuration);

        builder.Services.AddSwaggerWithAuth(builder.Configuration);

        builder.Services.AddIdentityService(builder.Configuration);

        builder.Services.AddDefaultAuthentication(builder.Configuration);

        builder.Services.AddTenantDb(builder.Configuration.GetConnectionString("TenantDb"));
        builder.Services.AddSessionService(builder.Configuration);

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
            app.UseSwaggerUIWithAuth();

            app.MapOpenApi();
        }

        app.UseCustomCors(builder.Configuration);

        app.UseConfiguredSentry();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers().RequireAuthorization();

        app.Run();
    }
}