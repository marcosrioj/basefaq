using Querify.Broadcast.Portal.Api.Extensions;
using Querify.Common.EntityFramework.Tenant.Extensions;
using Querify.Common.Infrastructure.ApiErrorHandling.Extensions;
using Querify.Common.Infrastructure.Core.Extensions;
using Querify.Common.Infrastructure.MediatR.Extensions;
using Querify.Common.Infrastructure.Mvc.Filters;
using Querify.Common.Infrastructure.Sentry.Extensions;
using Querify.Common.Infrastructure.Swagger.Extensions;
using Querify.Models.Common.Enums;

namespace Querify.Broadcast.Portal.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        builder.Services.AddOpenApi();
        builder.Services.AddCustomCors(builder.Configuration);
        builder.Services.AddSwaggerWithAuth(builder.Configuration);
        builder.Services.AddDefaultAuthentication(builder.Configuration);
        builder.Services.AddTenantDb(builder.Configuration.GetConnectionString("TenantDb"));
        builder.Services.AddSessionService(builder.Configuration);
        builder.Services.AddLogging(configuration =>
        {
            configuration.SetMinimumLevel(LogLevel.Information);
            configuration.AddConsole();
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddFeatures();
        builder.Services.AddMediatRLogging();
        builder.WebHost.AddConfiguredSentry(builder.Environment);
        builder.Services.AddControllers(options => options.Filters.Add(new StringTrimmingActionFilter()));

        var app = builder.Build();
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
        app.UseTenantResolution(ModuleEnum.Broadcast);
        app.MapControllers().RequireAuthorization();
        app.Run();
    }
}
