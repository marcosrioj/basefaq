using MediatR;
using Querify.Models.Direct.Dtos.Conversation;

namespace Querify.Direct.Portal.Business.Conversation.Commands.CreateConversation;

public sealed class ConversationsCreateCommand : IRequest<Guid>
{
    public required ConversationCreateRequestDto Request { get; set; }
}
