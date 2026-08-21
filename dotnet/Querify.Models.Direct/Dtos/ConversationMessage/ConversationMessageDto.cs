using Querify.Models.Direct.Enums;

namespace Querify.Models.Direct.Dtos.ConversationMessage;

public sealed class ConversationMessageDto
{
    public required Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid ConversationId { get; set; }
    public required MessageActorKind ActorKind { get; set; }
    public required string Body { get; set; }
    public required DateTime SentAtUtc { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
}
