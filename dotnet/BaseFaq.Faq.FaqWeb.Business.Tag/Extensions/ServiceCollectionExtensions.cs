using BaseFaq.Faq.FaqWeb.Business.Tag.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Tag.Commands.CreateTag;
using BaseFaq.Faq.FaqWeb.Business.Tag.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTagBusiness(this IServiceCollection services)
    {
        services.AddScoped<ITagService, TagService>();
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<TagsCreateTagCommandHandler>());

        return services;
    }
}