using Microsoft.Extensions.DependencyInjection;
using Querify.Broadcast.Portal.Business.Item.Abstractions;
using Querify.Broadcast.Portal.Business.Item.Commands.CreateItem;
using Querify.Broadcast.Portal.Business.Item.Service;

namespace Querify.Broadcast.Portal.Business.Item.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddItemBusiness(this IServiceCollection services)
    {
        services.AddScoped<IItemService, ItemService>();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<ItemsCreateCommandHandler>());
        return services;
    }
}
