using BaseFaq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.AI.Matching.Business.Matching.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.AI.Matching.Business.Matching.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMatchingBusiness(this IServiceCollection services)
    {
        services.AddScoped<IMatchingStatusService, MatchingStatusService>();
        services.AddScoped<IMatchingFaqItemValidationService, MatchingFaqItemValidationService>();
        services.AddScoped<IMatchingRequestService, MatchingRequestService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(MatchingStatusService).Assembly));

        return services;
    }
}