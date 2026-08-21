using Querify.Common.EntityFramework.Core.Abstractions;
using Querify.Common.EntityFramework.Core.Entities;

namespace Querify.Direct.Common.Domain.Entities;

/// <summary>
/// Represents the person or external participant served by Direct conversations.
/// </summary>
public class Contact : BaseEntity, IMustHaveTenant
{
    /// <summary>Maximum contact given-name length accepted by persistence.</summary>
    public const int MaxGivenNameLength = 100;

    /// <summary>Maximum contact surname length accepted by persistence.</summary>
    public const int MaxSurnameLength = 100;

    /// <summary>Maximum contact email address length accepted by persistence.</summary>
    public const int MaxEmailLength = 200;

    /// <summary>Maximum contact IANA time-zone identifier length accepted by persistence.</summary>
    public const int MaxTimeZoneLength = 100;

    /// <summary>Maximum contact photo URL length accepted by persistence.</summary>
    public const int MaxPhotoUrlLength = 1000;

    /// <summary>Maximum contact phone number length accepted by persistence.</summary>
    public const int MaxPhoneNumberLength = 200;

    /// <summary>Maximum Instagram profile URL length accepted by persistence.</summary>
    public const int MaxInstagramProfileUrlLength = 200;

    /// <summary>Maximum TikTok profile URL length accepted by persistence.</summary>
    public const int MaxTikTokProfileUrlLength = 200;

    /// <summary>Maximum Facebook profile URL length accepted by persistence.</summary>
    public const int MaxFacebookProfileUrlLength = 200;

    /// <summary>Maximum Snapchat profile URL length accepted by persistence.</summary>
    public const int MaxSnapchatProfileUrlLength = 200;

    /// <summary>
    /// Contact's given name used as the primary human-readable identity in Direct workflows.
    /// </summary>
    public required string GivenName { get; set; }

    /// <summary>
    /// Optional family name used with the given name when the contact provides it.
    /// </summary>
    public string? Surname { get; set; }

    /// <summary>
    /// Optional email address available for contact lookup and asynchronous communication.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Optional image URL used to display the contact consistently across Direct conversations.
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Optional IANA time zone used when presenting or scheduling contact-specific communication.
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Optional phone number used by phone-based channel connections such as SMS or WhatsApp.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Optional Instagram profile URL associated with this contact.
    /// </summary>
    public string? InstagramProfileUrl { get; set; }

    /// <summary>
    /// Optional TikTok profile URL associated with this contact.
    /// </summary>
    public string? TikTokProfileUrl { get; set; }

    /// <summary>
    /// Optional Facebook profile URL associated with this contact.
    /// </summary>
    public string? FacebookProfileUrl { get; set; }

    /// <summary>
    /// Optional Snapchat profile URL associated with this contact.
    /// </summary>
    public string? SnapchatProfileUrl { get; set; }

    /// <summary>
    /// Direct conversations associated with the contact inside the same tenant.
    /// </summary>
    public ICollection<Conversation> Conversations { get; set; } = [];

    /// <summary>
    /// Tenant that owns the contact and scopes all contact lookup and relationship behavior.
    /// </summary>
    public required Guid TenantId { get; set; }
}
