using Microsoft.Extensions.DependencyInjection;
using Querify.Direct.Portal.Business.ConversationMessage.Abstractions;
using Querify.Direct.Portal.Business.ConversationMessage.Commands.CreateConversationMessage;
using Querify.Direct.Portal.Business.ConversationMessage.Service;

namespace Querify.Direct.Portal.Business.ConversationMessage.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConversationMessageBusiness(this IServiceCollection services)
    {
        services.AddScoped<IConversationMessageService, ConversationMessageService>();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<ConversationMessagesCreateCommandHandler>());
        return services;
    }
}
