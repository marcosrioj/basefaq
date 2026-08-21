using Microsoft.Extensions.DependencyInjection;
using Querify.Direct.Portal.Business.Contact.Abstractions;
using Querify.Direct.Portal.Business.Contact.Commands.CreateContact;
using Querify.Direct.Portal.Business.Contact.Service;

namespace Querify.Direct.Portal.Business.Contact.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContactBusiness(this IServiceCollection services)
    {
        services.AddScoped<IContactService, ContactService>();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<ContactsCreateContactCommandHandler>());
        return services;
    }
}
