using BaseFaq.Faq.Portal.Business.FaqItem.Abstractions;
using BaseFaq.Faq.Portal.Business.FaqItem.Commands.CreateFaqItem;
using BaseFaq.Faq.Portal.Business.FaqItem.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.Portal.Business.FaqItem.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaqItemBusiness(this IServiceCollection services)
    {
        services.AddScoped<IFaqItemService, FaqItemService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<FaqItemsCreateFaqItemCommandHandler>());

        return services;
    }
}