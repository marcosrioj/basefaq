using BaseFaq.Tenant.TenantWeb.Business.User.Abstractions;
using BaseFaq.Tenant.TenantWeb.Business.User.Commands.CreateUser;
using BaseFaq.Tenant.TenantWeb.Business.User.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Tenant.TenantWeb.Business.User.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserBusiness(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<UsersCreateUserCommandHandler>());

        return services;
    }
}