using BaseFaq.Models.Tenant.Enums;

namespace BaseFaq.Models.Tenant.Dtos.User;

public class UserCreateRequestDto
{
    public required string GivenName { get; set; }
    public string? SurName { get; set; }
    public required string Email { get; set; }
    public required string ExternalId { get; set; }
    public string? PhoneNumber { get; set; }
    public required UserRoleType Role { get; set; }
}