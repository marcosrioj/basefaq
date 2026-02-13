using BaseFaq.AI.Generation.Business.Generation.Abstractions;
using BaseFaq.AI.Generation.Business.Generation.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.AI.Generation.Business.Generation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGenerationBusiness(this IServiceCollection services)
    {
        services.AddScoped<IGenerationStatusService, GenerationStatusService>();
        services.AddScoped<IGenerationRequestService, GenerationRequestService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(GenerationBusinessAssemblyMarker).Assembly));

        return services;
    }
}