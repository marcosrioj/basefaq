using Querify.Models.Direct.Dtos.Contact;

namespace Querify.Models.Direct.Dtos.Conversation;

public sealed class ConversationDetailDto : ConversationDto
{
    public required ContactDto Contact { get; set; }
}
