using Microsoft.Extensions.DependencyInjection;
using Querify.Direct.Portal.Business.Conversation.Abstractions;
using Querify.Direct.Portal.Business.Conversation.Commands.CreateConversation;
using Querify.Direct.Portal.Business.Conversation.Rules;
using Querify.Direct.Portal.Business.Conversation.Service;

namespace Querify.Direct.Portal.Business.Conversation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConversationBusiness(this IServiceCollection services)
    {
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<ChannelConnectionAccessValidator>();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<ConversationsCreateCommandHandler>());
        return services;
    }
}
