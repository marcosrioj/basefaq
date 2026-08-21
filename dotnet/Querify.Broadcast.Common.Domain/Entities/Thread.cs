using Querify.Common.EntityFramework.Core.Abstractions;
using Querify.Common.EntityFramework.Core.Entities;
using Querify.Models.Broadcast.Enums;

namespace Querify.Broadcast.Common.Domain.Entities;

/// <summary>
/// Represents a public or community interaction thread captured by Broadcast.
/// </summary>
public class Thread : BaseEntity, IMustHaveTenant
{
    /// <summary>Maximum optional Broadcast thread title length accepted by persistence.</summary>
    public const int MaxTitleLength = 1000;

    /// <summary>
    /// Current lifecycle state used to decide whether the thread remains active or has been completed.
    /// </summary>
    public required ThreadStatus Status { get; set; }

    /// <summary>
    /// Optional topic or provider title used for display and lookup when the source channel supplies one.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Tenant control-plane channel connection used to capture and respond to this public interaction.
    /// The ID is intentionally stored without an EF navigation because Tenant and Broadcast use separate databases.
    /// </summary>
    public required Guid ChannelConnectionId { get; set; }

    /// <summary>
    /// Broadcast items owned by this thread; tenant integrity is enforced through the parent relationship.
    /// </summary>
    public ICollection<Item> Items { get; set; } = [];

    /// <summary>
    /// Tenant that owns the thread and scopes tenant filters and relationship validation.
    /// </summary>
    public required Guid TenantId { get; set; }
}
