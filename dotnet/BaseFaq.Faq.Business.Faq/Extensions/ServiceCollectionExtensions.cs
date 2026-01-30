using BaseFaq.Faq.Business.Faq.Abstractions;
using BaseFaq.Faq.Business.Faq.Commands.CreateFaq;
using BaseFaq.Faq.Business.Faq.Service;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.Business.Faq.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaqBusiness(this IServiceCollection services)
    {
        services.AddScoped<IFaqService, FaqService>();
        services.AddScoped<IFaqItemService, FaqItemService>();
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<FaqsCreateFaqCommandHandler>());

        return services;
    }
}