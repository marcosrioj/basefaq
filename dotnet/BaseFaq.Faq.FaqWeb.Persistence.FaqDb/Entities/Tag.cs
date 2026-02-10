using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Core.Entities;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;

public class Tag : BaseEntity, IMustHaveTenant
{
    public const int MaxValueLength = 100;

    /// <summary>
    /// Normalized tag value used to identify or classify FAQs.
    /// Example: "returns", "delivery", "assembly".
    /// </summary>
    public required string Value { get; set; }

    public required Guid TenantId { get; set; }

    public ICollection<FaqTag> Faqs { get; set; } = [];
}