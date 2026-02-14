using BaseFaq.AI.Generation.Business.Generation.Commands.RequestGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.AI.Generation.Business.Generation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGenerationBusiness(this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(GenerationRequestCommand).Assembly));

        return services;
    }
}