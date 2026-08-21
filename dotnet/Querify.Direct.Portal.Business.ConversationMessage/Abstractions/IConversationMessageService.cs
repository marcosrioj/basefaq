using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.ConversationMessage;

namespace Querify.Direct.Portal.Business.ConversationMessage.Abstractions;

public interface IConversationMessageService
{
    Task<Guid> Create(ConversationMessageCreateRequestDto request, CancellationToken cancellationToken);
    Task<PagedResultDto<ConversationMessageDto>> GetAll(
        ConversationMessageGetAllRequestDto request,
        CancellationToken cancellationToken);
}
