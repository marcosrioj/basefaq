using Querify.Broadcast.Common.Persistence.BroadcastDb.Extensions;
using Querify.Broadcast.Portal.Business.Item.Extensions;
using Querify.Broadcast.Portal.Business.Thread.Extensions;

namespace Querify.Broadcast.Portal.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFeatures(this IServiceCollection services)
    {
        services.AddBroadcastDb();
        services.AddThreadBusiness();
        services.AddItemBusiness();
    }
}
