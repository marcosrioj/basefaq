using Querify.Common.EntityFramework.Core.Abstractions;
using Querify.Common.EntityFramework.Core.Entities;
using Querify.Models.Direct.Enums;

namespace Querify.Direct.Common.Domain.Entities;

/// <summary>
/// Represents a 1:1 support conversation owned by Direct.
/// </summary>
public class Conversation : BaseEntity, IMustHaveTenant
{
    /// <summary>Maximum optional conversation subject length accepted by persistence.</summary>
    public const int MaxSubjectLength = 500;

    /// <summary>
    /// Current lifecycle state used to decide whether the conversation is still active or already completed.
    /// </summary>
    public required ConversationStatus Status { get; set; }

    /// <summary>
    /// Optional human-readable topic supplied by the support channel; it is not required to identify the conversation.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Contact served by this conversation; the referenced contact must belong to the same Direct tenant.
    /// </summary>
    public required Guid ContactId { get; set; }

    /// <summary>
    /// Navigation to the contact used for Direct persistence and contact-scoped reads.
    /// </summary>
    public required Contact Contact { get; set; }

    /// <summary>
    /// Tenant control-plane channel connection used to receive and deliver this conversation.
    /// The ID is intentionally stored without an EF navigation because Tenant and Direct use separate databases.
    /// </summary>
    public required Guid ChannelConnectionId { get; set; }

    /// <summary>
    /// Messages owned by this conversation; tenant integrity is enforced through the parent relationship.
    /// </summary>
    public ICollection<ConversationMessage> Messages { get; set; } = [];

    /// <summary>
    /// Tenant that owns the conversation and scopes tenant filters and relationship validation.
    /// </summary>
    public required Guid TenantId { get; set; }
}
