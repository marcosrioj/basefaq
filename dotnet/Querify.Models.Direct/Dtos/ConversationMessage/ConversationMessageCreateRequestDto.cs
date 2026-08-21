using Querify.Models.Direct.Enums;

namespace Querify.Models.Direct.Dtos.ConversationMessage;

public sealed class ConversationMessageCreateRequestDto
{
    public required Guid ConversationId { get; set; }
    public required MessageActorKind ActorKind { get; set; }
    public required string Body { get; set; }
    public required DateTime SentAtUtc { get; set; }
}
