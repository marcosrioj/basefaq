using BaseFaq.Faq.FaqWeb.Business.Faq.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaq;
using BaseFaq.Faq.FaqWeb.Business.Faq.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Extensions;

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