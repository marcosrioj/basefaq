using Querify.Models.Broadcast.Enums;

namespace Querify.Models.Broadcast.Dtos.Thread;

public sealed class ThreadCreateRequestDto
{
    public required Guid ChannelConnectionId { get; set; }
    public string? Title { get; set; }
    public required ThreadStatus Status { get; set; }
}
