using BaseFaq.Tenant.TenantWeb.Business.Abstractions;
using BaseFaq.Tenant.TenantWeb.Business.Commands.CreateTenant;
using BaseFaq.Tenant.TenantWeb.Business.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Tenant.TenantWeb.Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTenantBusiness(this IServiceCollection services)
    {
        services.AddScoped<ITenantConnectionService, TenantConnectionService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IUserService, UserService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<TenantsCreateTenantCommandHandler>());

        return services;
    }
}