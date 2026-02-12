using BaseFaq.Tenant.Portal.Business.User.Abstractions;
using BaseFaq.Tenant.Portal.Business.User.Commands.UpdateUserProfile;
using BaseFaq.Tenant.Portal.Business.User.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Tenant.Portal.Business.User.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserBusiness(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<UsersUpdateUserProfileCommandHandler>());

        return services;
    }
}