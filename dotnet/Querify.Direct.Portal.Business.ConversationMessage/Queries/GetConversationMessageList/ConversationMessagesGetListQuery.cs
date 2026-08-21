using MediatR;
using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.ConversationMessage;

namespace Querify.Direct.Portal.Business.ConversationMessage.Queries.GetConversationMessageList;

public sealed class ConversationMessagesGetListQuery : IRequest<PagedResultDto<ConversationMessageDto>>
{
    public required ConversationMessageGetAllRequestDto Request { get; set; }
}
