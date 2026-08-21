using Microsoft.Extensions.DependencyInjection;
using Querify.Tenant.BackOffice.Business.ChannelConnection.Abstractions;
using Querify.Tenant.BackOffice.Business.ChannelConnection.Commands.UpdateOperationalState;
using Querify.Tenant.BackOffice.Business.ChannelConnection.Service;

namespace Querify.Tenant.BackOffice.Business.ChannelConnection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChannelConnectionBusiness(this IServiceCollection services)
    {
        services.AddScoped<IChannelConnectionBackOfficeService, ChannelConnectionBackOfficeService>();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<ChannelConnectionsUpdateOperationalStateCommandHandler>());
        return services;
    }
}
