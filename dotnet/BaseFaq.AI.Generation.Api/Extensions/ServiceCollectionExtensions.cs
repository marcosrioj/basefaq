using BaseFaq.AI.Generation.Business.Generation.Extensions;
using BaseFaq.AI.Generation.Business.Worker.Extensions;
using BaseFaq.AI.Common.Persistence.AiDb.Extensions;

namespace BaseFaq.AI.Generation.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAiDb(configuration.GetConnectionString("AiDb"));
        services.AddGenerationBusiness();
        services.AddGenerationWorker(configuration);
    }
}