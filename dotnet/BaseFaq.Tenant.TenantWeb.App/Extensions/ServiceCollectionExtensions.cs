using BaseFaq.Tenant.TenantWeb.Business.Tenant.Extensions;
using BaseFaq.Tenant.TenantWeb.Business.User.Extensions;

namespace BaseFaq.Tenant.TenantWeb.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTenantBusiness();
        services.AddUserBusiness();
        //services.AddEventsFeature();
    }
}