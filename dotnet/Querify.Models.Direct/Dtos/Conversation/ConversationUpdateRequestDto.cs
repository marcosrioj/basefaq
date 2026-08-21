using Querify.Models.Direct.Enums;

namespace Querify.Models.Direct.Dtos.Conversation;

public sealed class ConversationUpdateRequestDto
{
    public required Guid ContactId { get; set; }
    public required Guid ChannelConnectionId { get; set; }
    public string? Subject { get; set; }
    public required ConversationStatus Status { get; set; }
}
