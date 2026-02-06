using BaseFaq.Faq.FaqWeb.Business.Faq.Extensions;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Extensions;

namespace BaseFaq.Faq.FaqWeb.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFaqDb(configuration.GetConnectionString("FaqDb"));
        services.AddFaqBusiness();
        //services.AddEventsFeature();
    }
}