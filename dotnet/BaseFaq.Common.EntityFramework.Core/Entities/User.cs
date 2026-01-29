using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Core.Entities.Base;
using BaseFaq.Models.Enums;

namespace BaseFaq.Common.EntityFramework.Core.Entities;

public class User : BaseEntity, IMayHaveTenant
{
    public required string GivenName { get; set; }
    public required string SurName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRoleType Role { get; set; }

    public Guid? TenantId { get; set; }
}