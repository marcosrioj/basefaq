using BaseFaq.Common.Infrastructure.MediatR.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Common.Infrastructure.MediatR.Extensions;

public static class MediatRLoggingServiceCollectionExtension
{
    public static void AddMediatRLogging(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    }
}