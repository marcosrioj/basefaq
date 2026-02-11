using BaseFaq.Tenant.Portal.Business.Tenant.Abstractions;
using BaseFaq.Tenant.Portal.Business.Tenant.Commands.CreateTenant;
using BaseFaq.Tenant.Portal.Business.Tenant.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Tenant.Portal.Business.Tenant.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTenantBusiness(this IServiceCollection services)
    {
        services.AddScoped<ITenantConnectionService, TenantConnectionService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<TenantsCreateTenantCommandHandler>());

        return services;
    }
}