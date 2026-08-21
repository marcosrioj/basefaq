using Querify.Models.Tenant.Enums;

namespace Querify.Models.Tenant.Dtos.ChannelConnection;

public sealed class ChannelConnectionDto
{
    public required Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required string Name { get; set; }
    public required string ProviderKey { get; set; }
    public required ChannelConnectionKind Kind { get; set; }
    public required ChannelConnectionStatus Status { get; set; }
    public required bool IsEnabled { get; set; }
    public DateTime? CredentialsExpireAtUtc { get; set; }
    public DateTime? LastCredentialsRefreshAtUtc { get; set; }
    public DateTime? LastConnectedAtUtc { get; set; }
    public DateTime? LastSynchronizedAtUtc { get; set; }
    public DateTime? LastErrorAtUtc { get; set; }
    public string? LastErrorMessage { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? LastUpdatedAtUtc { get; set; }
}
