using System.ComponentModel.DataAnnotations;

namespace BaseFaq.Mas.Common.Persistence.MasDb.Base;

public class BaseEntity : AuditableEntity
{
    [Key] public Guid Id { get; set; }
}