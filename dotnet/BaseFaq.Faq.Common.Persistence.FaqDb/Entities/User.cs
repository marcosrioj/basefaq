using BaseFaq.Faq.Common.Persistence.FaqDb.Base;
using BaseFaq.Models.Enums;

namespace BaseFaq.Faq.Common.Persistence.FaqDb.Entities;

public class User : BaseEntity
{
    public required string GivenName { get; set; }
    public required string SurName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRoleType Role { get; set; }

    public Guid? TenantId { get; set; }
}