namespace BaseFaq.Tenant.Portal.Business.User.Dtos;

public class UserProfileDto
{
    public required string GivenName { get; set; }
    public string? SurName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
}