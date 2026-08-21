using Querify.Common.EntityFramework.Core.Abstractions;
using Querify.Common.EntityFramework.Core.Entities;
using Querify.Models.Tenant.Enums;

namespace Querify.Common.EntityFramework.Tenant.Entities;

/// <summary>
/// Represents a tenant-owned provider connection that enables Direct or Broadcast channel workflows.
/// </summary>
public class ChannelConnection : BaseEntity, IMustHaveTenant
{
    /// <summary>Maximum operator-facing connection name length accepted by persistence.</summary>
    public const int MaxNameLength = 120;

    /// <summary>Maximum external provider endpoint identifier length accepted by persistence.</summary>
    public const int MaxProviderKeyLength = 200;

    /// <summary>Maximum encrypted connector configuration payload length accepted by persistence.</summary>
    public const int MaxConnectionDataLength = 16000;

    /// <summary>Maximum sanitized operational error summary length accepted by persistence.</summary>
    public const int MaxLastErrorMessageLength = 2000;

    /// <summary>
    /// Operator-facing name used to distinguish multiple connections of the same channel kind.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Provider account, inbox, page, bot, or phone identifier used to distinguish the external endpoint.
    /// </summary>
    public required string ProviderKey { get; set; }

    /// <summary>
    /// Provider surface that determines which connector behavior can use this connection.
    /// </summary>
    public required ChannelConnectionKind Kind { get; set; }

    /// <summary>
    /// Encrypted JSON payload containing provider credentials and connector-specific configuration.
    /// This value is write-only at the API boundary and must never be returned by read DTOs.
    /// </summary>
    public required string ConnectionData { get; set; }

    /// <summary>
    /// Current provider authorization and operational state used to gate connector workflows.
    /// </summary>
    public required ChannelConnectionStatus Status { get; set; }

    /// <summary>
    /// Independent tenant switch that allows operators to pause connector use without deleting its configuration.
    /// </summary>
    public required bool IsEnabled { get; set; }

    /// <summary>
    /// UTC instant when the current provider credentials expire, when the provider reports an expiry.
    /// </summary>
    public DateTime? CredentialsExpireAtUtc { get; set; }

    /// <summary>
    /// UTC instant when provider credentials were last refreshed successfully.
    /// </summary>
    public DateTime? LastCredentialsRefreshAtUtc { get; set; }

    /// <summary>
    /// UTC instant when Querify last established a successful provider connection.
    /// </summary>
    public DateTime? LastConnectedAtUtc { get; set; }

    /// <summary>
    /// UTC instant when the connector last completed synchronization successfully.
    /// </summary>
    public DateTime? LastSynchronizedAtUtc { get; set; }

    /// <summary>
    /// UTC instant when the connector last recorded an operational failure.
    /// </summary>
    public DateTime? LastErrorAtUtc { get; set; }

    /// <summary>
    /// Sanitized diagnostic summary for the latest connector failure; it must not contain credentials or tokens.
    /// </summary>
    public string? LastErrorMessage { get; set; }

    /// <summary>
    /// Base QnA tenant that owns the workspace-level connection in the Tenant control plane.
    /// </summary>
    public required Guid TenantId { get; set; }
}
