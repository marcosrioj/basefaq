using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Core.Entities;
using BaseFaq.Models.Faq.Enums;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;

public class ContentRef : BaseEntity, IMustHaveTenant
{
    public const int MaxLocatorLength = 1000;
    public const int MaxLabelLength = 200;
    public const int MaxScopeLength = 1000;

    /// <summary>
    /// Type of the base content (web, pdf, manual, video, etc.).
    /// Used to classify how the content was sourced.
    /// </summary>
    public ContentRefKind Kind { get; set; }

    /// <summary>
    /// Primary locator of the content source.
    /// Can be a URL, file identifier, path, document ID, or any unique pointer.
    /// </summary>
    public required string Locator { get; set; } = null!;

    /// <summary>
    /// Human-readable label or title for the content source.
    /// Used for identification, auditing, and admin visibility.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Optional scope within the content.
    /// Identifies a specific part such as page, section, paragraph, or time range.
    /// </summary>
    public string? Scope { get; set; }

    public required Guid TenantId { get; set; }

    public ICollection<FaqContentRef> Faqs { get; set; } = [];
}