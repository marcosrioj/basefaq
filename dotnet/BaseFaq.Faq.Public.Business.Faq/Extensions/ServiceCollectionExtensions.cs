using BaseFaq.Faq.Public.Business.Faq.Abstractions;
using BaseFaq.Faq.Public.Business.Faq.Queries.GetFaqList;
using BaseFaq.Faq.Public.Business.Faq.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.Public.Business.Faq.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaqBusiness(this IServiceCollection services)
    {
        services.AddScoped<IFaqService, FaqService>();
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<FaqsGetFaqListQueryHandler>());

        return services;
    }
}