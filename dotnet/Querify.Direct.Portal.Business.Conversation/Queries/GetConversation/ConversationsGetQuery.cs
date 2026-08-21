using MediatR;
using Querify.Models.Direct.Dtos.Conversation;

namespace Querify.Direct.Portal.Business.Conversation.Queries.GetConversation;

public sealed class ConversationsGetQuery : IRequest<ConversationDetailDto>
{
    public required Guid Id { get; set; }
}
