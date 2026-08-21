using MediatR;
using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.Conversation;

namespace Querify.Direct.Portal.Business.Conversation.Queries.GetConversationList;

public sealed class ConversationsGetListQuery : IRequest<PagedResultDto<ConversationDto>>
{
    public required ConversationGetAllRequestDto Request { get; set; }
}
