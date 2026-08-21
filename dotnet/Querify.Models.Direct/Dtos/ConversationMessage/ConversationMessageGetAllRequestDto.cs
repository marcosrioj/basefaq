using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Enums;

namespace Querify.Models.Direct.Dtos.ConversationMessage;

public sealed class ConversationMessageGetAllRequestDto : PagedAndSortedResultRequestDto
{
    public required Guid ConversationId { get; set; }
    public MessageActorKind? ActorKind { get; set; }
}
