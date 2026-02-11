using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Core.Entities;

namespace BaseFaq.Faq.Common.Persistence.FaqDb.Entities;

public class FaqContentRef : BaseEntity, IMustHaveTenant
{
    public required Guid TenantId { get; set; }

    public required Guid FaqId { get; set; }
    public Faq Faq { get; set; } = null!;

    public required Guid ContentRefId { get; set; }
    public ContentRef ContentRef { get; set; } = null!;
}