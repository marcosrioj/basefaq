using Querify.Models.Broadcast.Enums;

namespace Querify.Models.Broadcast.Dtos.Thread;

public class ThreadDto
{
    public required Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid ChannelConnectionId { get; set; }
    public string? Title { get; set; }
    public required ThreadStatus Status { get; set; }
    public required int ItemCount { get; set; }
    public DateTime? LastItemAtUtc { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? LastUpdatedAtUtc { get; set; }
}
