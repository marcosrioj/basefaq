using BaseFaq.Faq.FaqWeb.Business.FaqItem.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.FaqItem.Commands.CreateFaqItem;
using BaseFaq.Faq.FaqWeb.Business.FaqItem.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.FaqWeb.Business.FaqItem.Extensions;

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