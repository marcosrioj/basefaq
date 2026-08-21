using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Enums;

namespace Querify.Models.Direct.Dtos.Conversation;

public sealed class ConversationGetAllRequestDto : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? ChannelConnectionId { get; set; }
    public ConversationStatus? Status { get; set; }
}
