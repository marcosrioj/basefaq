using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Services;

namespace BaseFaq.Faq.FaqWeb.App.Extensions;

public static class ServiceCollectionExtensions
{
    // public static void AddFeatures(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddUserProfile(configuration);
    //     services.AddEventsFeature();
    //     services.AddCommonOperation();
    //     services.AddPermissions();
    // }

    public static void AddIdentityService(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IIdentityService, IdentityService>();
    }
}