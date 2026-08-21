using Microsoft.Extensions.DependencyInjection;
using Querify.Broadcast.Portal.Business.Thread.Abstractions;
using Querify.Broadcast.Portal.Business.Thread.Commands.CreateThread;
using Querify.Broadcast.Portal.Business.Thread.Rules;
using Querify.Broadcast.Portal.Business.Thread.Service;

namespace Querify.Broadcast.Portal.Business.Thread.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddThreadBusiness(this IServiceCollection services)
    {
        services.AddScoped<IThreadService, ThreadService>();
        services.AddScoped<ChannelConnectionAccessValidator>();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<ThreadsCreateCommandHandler>());
        return services;
    }
}
