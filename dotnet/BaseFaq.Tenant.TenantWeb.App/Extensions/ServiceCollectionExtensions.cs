using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Services;
using BaseFaq.Tenant.TenantWeb.Business.Extensions;

namespace BaseFaq.Tenant.TenantWeb.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTenantBusiness();
        //services.AddEventsFeature();
    }

    public static void AddClaimService(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IClaimService, ClaimService>();
    }
}