using System.ComponentModel.DataAnnotations;

namespace BaseFaq.Faq.Common.Persistence.FaqDb.Base;

public class BaseEntity : AuditableEntity
{
    [Key] public Guid Id { get; set; }
}