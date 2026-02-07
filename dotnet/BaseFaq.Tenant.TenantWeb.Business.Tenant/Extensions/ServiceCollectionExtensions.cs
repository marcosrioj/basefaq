using BaseFaq.Tenant.TenantWeb.Business.Tenant.Abstractions;
using BaseFaq.Tenant.TenantWeb.Business.Tenant.Commands.CreateTenant;
using BaseFaq.Tenant.TenantWeb.Business.Tenant.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Tenant.TenantWeb.Business.Tenant.Extensions;

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