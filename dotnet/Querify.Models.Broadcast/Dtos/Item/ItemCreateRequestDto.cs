using Querify.Models.Broadcast.Enums;

namespace Querify.Models.Broadcast.Dtos.Item;

public sealed class ItemCreateRequestDto
{
    public required Guid ThreadId { get; set; }
    public required ItemKind Kind { get; set; }
    public required ActorKind ActorKind { get; set; }
    public required string Body { get; set; }
    public required DateTime CapturedAtUtc { get; set; }
}
