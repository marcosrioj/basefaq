using Querify.Common.EntityFramework.Core.Abstractions;
using Querify.Common.EntityFramework.Core.Entities;
using Querify.Models.Direct.Enums;
using Querify.Models.Tenant.Enums;

namespace Querify.Common.EntityFramework.Tenant.Entities;

public class ChannelConnection : BaseEntity, IMustHaveTenant
{
    // Basic information
    public required string Name { get; set; }

    public required string ProviderKey { get; set; }

    public required ChannelConnectionKind Kind { get; set; }

    // Encrypted provider credentials and configuration
    public string ConnectionData { get; set; } = "{}";

    // Status
    public required ChannelConnectionStatus Status { get; set; }
        = ChannelConnectionStatus.Pending;

    public required bool IsEnabled { get; set; } = true;

    // Credential expiration and refresh
    public DateTimeOffset? CredentialsExpireAt { get; set; }

    public DateTimeOffset? LastCredentialsRefreshAt { get; set; }

    // Monitoring
    public DateTimeOffset? LastConnectedAt { get; set; }

    public DateTimeOffset? LastSynchronizedAt { get; set; }

    public DateTimeOffset? LastErrorAt { get; set; }

    public string? LastErrorMessage { get; set; }

    /// <summary>
    /// Tenant that owns the item and must match the owning thread tenant.
    /// </summary>
    public required Guid TenantId { get; set; }
}