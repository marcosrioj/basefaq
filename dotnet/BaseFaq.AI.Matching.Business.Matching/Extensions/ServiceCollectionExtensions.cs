using BaseFaq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.AI.Matching.Business.Matching.Commands.RequestMatching;
using BaseFaq.AI.Matching.Business.Matching.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.AI.Matching.Business.Matching.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMatchingBusiness(this IServiceCollection services)
    {
        services.AddScoped<IMatchingFaqItemValidationService, MatchingFaqItemValidationService>();
        services.AddScoped<IMatchingRequestPublisher, MatchingRequestPublisher>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(MatchingRequestCommand).Assembly));

        return services;
    }
}