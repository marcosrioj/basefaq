using BaseFaq.Tenant.TenantWeb.Business.Extensions;

namespace BaseFaq.Tenant.TenantWeb.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTenantBusiness();
        //services.AddEventsFeature();
    }
}