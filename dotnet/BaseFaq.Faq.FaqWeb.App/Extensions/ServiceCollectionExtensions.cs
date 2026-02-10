using BaseFaq.Faq.FaqWeb.Business.ContentRef.Extensions;
using BaseFaq.Faq.FaqWeb.Business.Faq.Extensions;
using BaseFaq.Faq.FaqWeb.Business.FaqItem.Extensions;
using BaseFaq.Faq.FaqWeb.Business.Tag.Extensions;
using BaseFaq.Faq.FaqWeb.Business.Vote.Extensions;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Extensions;

namespace BaseFaq.Faq.FaqWeb.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFaqDb();
        services.AddFaqBusiness();
        services.AddFaqItemBusiness();
        services.AddTagBusiness();
        services.AddContentRefBusiness();
        services.AddVoteBusiness();
        //services.AddEventsFeature();
    }
}