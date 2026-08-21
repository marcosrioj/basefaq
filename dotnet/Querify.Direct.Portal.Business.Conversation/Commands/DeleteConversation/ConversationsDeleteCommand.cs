using MediatR;

namespace Querify.Direct.Portal.Business.Conversation.Commands.DeleteConversation;

public sealed class ConversationsDeleteCommand : IRequest
{
    public required Guid Id { get; set; }
}
