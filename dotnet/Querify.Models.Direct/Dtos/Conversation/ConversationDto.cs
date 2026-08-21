using Querify.Models.Direct.Enums;

namespace Querify.Models.Direct.Dtos.Conversation;

public class ConversationDto
{
    public required Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid ContactId { get; set; }
    public required Guid ChannelConnectionId { get; set; }
    public string? Subject { get; set; }
    public required ConversationStatus Status { get; set; }
    public required int MessageCount { get; set; }
    public DateTime? LastMessageAtUtc { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? LastUpdatedAtUtc { get; set; }
}
