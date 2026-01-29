using System.ComponentModel.DataAnnotations;

namespace BaseFaq.Common.EntityFramework.Core.Entities.Base;

public class BaseEntity : AuditableEntity
{
    [Key] public Guid Id { get; set; }
}