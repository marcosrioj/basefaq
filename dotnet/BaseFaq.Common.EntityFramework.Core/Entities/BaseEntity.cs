using System.ComponentModel.DataAnnotations;

namespace BaseFaq.Common.EntityFramework.Core.Entities;

public class BaseEntity : AuditableEntity
{
    [Key] public Guid Id { get; set; }
}