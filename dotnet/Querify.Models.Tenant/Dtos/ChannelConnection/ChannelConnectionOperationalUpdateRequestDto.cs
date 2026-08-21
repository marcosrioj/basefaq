using Querify.Models.Tenant.Enums;

namespace Querify.Models.Tenant.Dtos.ChannelConnection;

public sealed class ChannelConnectionOperationalUpdateRequestDto
{
    public required ChannelConnectionStatus Status { get; set; }
    public DateTime? CredentialsExpireAtUtc { get; set; }
    public DateTime? LastCredentialsRefreshAtUtc { get; set; }
    public DateTime? LastConnectedAtUtc { get; set; }
    public DateTime? LastSynchronizedAtUtc { get; set; }
    public DateTime? LastErrorAtUtc { get; set; }
    public string? LastErrorMessage { get; set; }
}
