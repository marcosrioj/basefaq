using Querify.Common.EntityFramework.Core.Entities;
using Querify.Models.Common.Enums;
using Querify.Models.Tenant.Enums;

namespace Querify.Common.EntityFramework.Tenant.Entities;

/// <summary>
/// Represents one module-specific runtime boundary inside a Querify workspace.
/// </summary>
public class Tenant : BaseEntity
{
    /// <summary>Maximum module-specific tenant slug length accepted by persistence.</summary>
    public const int MaxSlugLength = 128;

    /// <summary>Maximum workspace display name length accepted by persistence.</summary>
    public const int MaxNameLength = 128;

    /// <summary>Maximum encrypted module database connection string length accepted by persistence.</summary>
    public const int MaxConnectionStringLength = 1024;

    /// <summary>Maximum public module client key length accepted by persistence.</summary>
    public const int MaxClientKeyLength = 128;

    /// <summary>Default display name used when no explicit workspace name is available.</summary>
    public const string DefaultTenantName = "Default";

    /// <summary>
    /// URL-safe module-specific identifier used in administrative and routing contexts.
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// Workspace display name shared by every module tenant in the same workspace.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Commercial edition that controls the workspace's available capabilities.
    /// </summary>
    public required TenantEdition Edition { get; set; }

    /// <summary>
    /// Stable workspace identifier shared by the QnA, Direct, Broadcast, and Trust tenant records.
    /// </summary>
    public required Guid WorkspaceId { get; set; }

    /// <summary>
    /// Querify module that owns this tenant runtime record and its module database connection.
    /// </summary>
    public required ModuleEnum Module { get; set; }

    /// <summary>
    /// Encrypted module database connection string resolved for requests to this tenant.
    /// </summary>
    public required string ConnectionString { get; set; }

    /// <summary>
    /// Optional public API key used only by module surfaces that support client-key access.
    /// </summary>
    public string? ClientKey { get; set; }

    /// <summary>
    /// Indicates whether the module tenant can be selected and used by workspace members.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// User memberships that grant access to this module tenant.
    /// </summary>
    public ICollection<TenantUser> TenantUsers { get; set; } = [];
}
