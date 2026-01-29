using BaseFaq.Common.EntityFramework.Core.Entities;
using BaseFaq.Common.EntityFramework.Tenant.Helpers;
using BaseFaq.Models.Enums;

namespace BaseFaq.Common.EntityFramework.Tenant.Entities;

public class Tenant : BaseEntity
{
    public const int MaxSlugLength = 128;
    public const int MaxNameLength = 128;
    public const int MaxConnectionStringLength = 1024;
    public const string DefaultTenantName = "Default";

    public required string Slug { get; set; } = TenantHelper.GenerateSlug(DefaultTenantName);
    public required string Name { get; set; } = DefaultTenantName;
    public required TenantEdition Edition { get; set; }
    public required string ConnectionString { get; set; }
    public bool IsActive { get; set; } = true;
}