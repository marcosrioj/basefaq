using BaseFaq.Faq.Portal.Business.ContentRef.Abstractions;
using BaseFaq.Faq.Portal.Business.ContentRef.Commands.CreateContentRef;
using BaseFaq.Faq.Portal.Business.ContentRef.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.Portal.Business.ContentRef.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContentRefBusiness(this IServiceCollection services)
    {
        services.AddScoped<IContentRefService, ContentRefService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<ContentRefsCreateContentRefCommandHandler>());

        return services;
    }
}