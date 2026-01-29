using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Core.Entities.Base;
using BaseFaq.Models.Enums;

namespace BaseFaq.Faq.Persistence.FaqDb.Entities;

public class Faq : BaseEntity, IMustHaveTenant
{
    public required string Name { get; set; }
    public required string Language { get; set; }
    public required FaqStatus Status { get; set; }
    public required FaqSortType SortType { get; set; }

    public required Guid TenantId { get; set; }

    public ICollection<FaqItem> Items { get; set; } = [];
}