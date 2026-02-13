using BaseFaq.AI.Common.Persistence.AiDb.Extensions;

namespace BaseFaq.AI.Matching.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAiDb(configuration.GetConnectionString("AiDb"));
    }
}