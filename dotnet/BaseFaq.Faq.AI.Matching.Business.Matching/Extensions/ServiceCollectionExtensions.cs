using BaseFaq.Faq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.Faq.AI.Matching.Business.Matching.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.AI.Matching.Business.Matching.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMatchingBusiness(this IServiceCollection services)
    {
        services.AddScoped<IMatchingStatusService, MatchingStatusService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(MatchingBusinessAssemblyMarker).Assembly));

        return services;
    }
}