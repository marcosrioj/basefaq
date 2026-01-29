using BaseFaq.Faq.Common.Persistence.FaqDb.Base;
using BaseFaq.Models.Enums;
using BaseFaq.Models.Enums.Helpers;

namespace BaseFaq.Faq.Common.Persistence.FaqDb.Entities;

public class Faq : BaseEntity
{
    public required string Name { get; set; }
    public required string Language { get; set; }
    public required FaqStatus Status { get; set; }
    public required FaqSortType SortType { get; set; }

    public required Guid TenantId { get; set; }

    public ICollection<FaqItem> Items { get; set; } = [];
}