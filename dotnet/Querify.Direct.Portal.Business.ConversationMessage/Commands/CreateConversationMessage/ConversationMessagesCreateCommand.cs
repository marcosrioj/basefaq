using MediatR;
using Querify.Models.Direct.Dtos.ConversationMessage;

namespace Querify.Direct.Portal.Business.ConversationMessage.Commands.CreateConversationMessage;

public sealed class ConversationMessagesCreateCommand : IRequest<Guid>
{
    public required ConversationMessageCreateRequestDto Request { get; set; }
}
