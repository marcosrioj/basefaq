using Querify.Common.EntityFramework.Core.Abstractions;
using Querify.Common.EntityFramework.Core.Entities;

namespace Querify.Direct.Common.Domain.Entities;

public class Contact : BaseEntity, IMustHaveTenant
{
    public const int MaxGivenNameLength = 100;
    public const int MaxSurNameLength = 100;
    public const int MaxEmailLength = 200;
    public const int MaxTimeZoneLength = 100;
    public const int MaxPhotoUrlLength = 1000;
    public const int MaxPhoneNumberLength = 200;
    public const int MaxInstagramProfileUrlLength = 200;
    public const int MaxTiktokProfileUrlLength = 200;
    public const int MaxFacebookProfileUrlLength = 200;
    public const int MaxSnapchatProfileUrlLength = 200;

    public required string GivenName { get; set; }
    public string? SurName { get; set; }
    public string? Email { get; set; }
    public string? PhotoUrl { get; set; }
    public string? TimeZone { get; set; }
    public string? PhoneNumber { get; set; }
    public string? InstagramProfileUrl { get; set; }
    public string? TiktokProfileUrl { get; set; }
    public string? FacebookProfileUrl { get; set; }
    public string? SnapchatProfileUrl { get; set; }

    /// <summary>
    /// Tenant that owns the conversation and scopes tenant filters and relationship validation.
    /// </summary>
    public required Guid TenantId { get; set; }
}