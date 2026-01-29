using System.ComponentModel.DataAnnotations;
using BaseFaq.Faq.Common.Persistence.FaqDb.Base;
using BaseFaq.Faq.Common.Persistence.FaqDb.Helpers;
using BaseFaq.Faq.Common.Persistence.FaqDb.Security;
using BaseFaq.Models.Enums;

namespace BaseFaq.Faq.Common.Persistence.FaqDb.Entities;

public class Tenant : BaseEntity
{
    public const int MaxSlugLength = 64;
    public const int MaxNameLength = 128;
    public const int MaxConnectionStringLength = 1024;
    public const string DefaultTenantName = "Default";

    [Required]
    [StringLength(MaxSlugLength)]
    public required string Slug { get; set; } = TenantHelper.GenerateSlug(DefaultTenantName);

    [Required]
    [StringLength(MaxNameLength)]
    public required string Name { get; set; } = DefaultTenantName;

    [Required] public required TenantEdition Edition { get; set; }

    /// <summary>
    /// ENCRYPTED connection string of the tenant database.
    /// Use <see cref="StringCipher"/> to encrypt/decrypt this.
    /// </summary>
    [Required]
    [StringLength(MaxConnectionStringLength)]
    public required string ConnectionString { get; set; }

    [Required] public bool IsActive { get; set; } = true;
}