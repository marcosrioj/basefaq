using Querify.Direct.Common.Persistence.DirectDb.Extensions;
using Querify.Direct.Portal.Business.Contact.Extensions;
using Querify.Direct.Portal.Business.Conversation.Extensions;
using Querify.Direct.Portal.Business.ConversationMessage.Extensions;

namespace Querify.Direct.Portal.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFeatures(this IServiceCollection services)
    {
        services.AddDirectDb();
        services.AddContactBusiness();
        services.AddConversationBusiness();
        services.AddConversationMessageBusiness();
    }
}
