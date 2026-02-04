using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Core.Entities;
using BaseFaq.Models.Tenant.Enums;

namespace BaseFaq.Common.EntityFramework.Tenant.Entities;

public class User : BaseEntity, IMustHaveTenant
{
    public const int MaxGivenNameLength = 100;
    public const int MaxSurNameLength = 100;
    public const int MaxEmailLength = 200;
    public const int MaxPhoneNumberLength = 200;
    public const int MaxExternalIdLength = 200;

    public required string GivenName { get; set; }
    public string? SurName { get; set; }
    public required string Email { get; set; }
    public required string ExternalId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRoleType Role { get; set; }

    public required Guid TenantId { get; set; }
}