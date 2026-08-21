using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.Conversation;

namespace Querify.Direct.Portal.Business.Conversation.Abstractions;

public interface IConversationService
{
    Task<Guid> Create(ConversationCreateRequestDto request, CancellationToken cancellationToken);
    Task<Guid> Update(Guid id, ConversationUpdateRequestDto request, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
    Task<ConversationDetailDto> GetById(Guid id, CancellationToken cancellationToken);
    Task<PagedResultDto<ConversationDto>> GetAll(
        ConversationGetAllRequestDto request,
        CancellationToken cancellationToken);
}
