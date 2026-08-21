using Microsoft.Extensions.DependencyInjection;
using Querify.Tenant.Portal.Business.ChannelConnection.Abstractions;
using Querify.Tenant.Portal.Business.ChannelConnection.Commands.CreateChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Rules;
using Querify.Tenant.Portal.Business.ChannelConnection.Service;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChannelConnectionBusiness(this IServiceCollection services)
    {
        services.AddScoped<IChannelConnectionService, ChannelConnectionService>();
        services.AddScoped<ChannelConnectionTenantResolver>();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<ChannelConnectionsCreateCommandHandler>());
        return services;
    }
}
