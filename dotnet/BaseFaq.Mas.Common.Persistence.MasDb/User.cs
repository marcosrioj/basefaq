using BaseFaq.Mas.Common.Persistence.MasDb.Base;
using BaseFaq.Models.Enums;

namespace BaseFaq.Mas.Common.Persistence.MasDb;

public class User : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRoleType Role { get; set; }

    public ICollection<Address> Addresses { get; set; } = [];
}