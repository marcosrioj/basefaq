using BaseFaq.Faq.Public.Business.FaqItem.Abstractions;
using BaseFaq.Faq.Public.Business.FaqItem.Queries.SearchFaqItem;
using BaseFaq.Faq.Public.Business.FaqItem.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.Public.Business.FaqItem.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaqItemBusiness(this IServiceCollection services)
    {
        services.AddScoped<IFaqItemService, FaqItemService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<FaqItemsSearchFaqItemQueryHandler>());

        return services;
    }
}