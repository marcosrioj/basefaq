using BaseFaq.Faq.Portal.Business.Faq.Abstractions;
using BaseFaq.Faq.Portal.Business.Faq.Commands.CreateFaq;
using BaseFaq.Faq.Portal.Business.Faq.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.Portal.Business.Faq.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaqBusiness(this IServiceCollection services)
    {
        services.AddScoped<IFaqService, FaqService>();
        services.AddScoped<IFaqTagService, FaqTagService>();
        services.AddScoped<IFaqContentRefService, FaqContentRefService>();
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<FaqsCreateFaqCommandHandler>());

        return services;
    }
}