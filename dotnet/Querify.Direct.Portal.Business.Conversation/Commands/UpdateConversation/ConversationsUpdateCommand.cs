using MediatR;
using Querify.Models.Direct.Dtos.Conversation;

namespace Querify.Direct.Portal.Business.Conversation.Commands.UpdateConversation;

public sealed class ConversationsUpdateCommand : IRequest<Guid>
{
    public required Guid Id { get; set; }
    public required ConversationUpdateRequestDto Request { get; set; }
}
