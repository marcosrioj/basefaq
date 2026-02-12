namespace BaseFaq.Tenant.Portal.Business.User.Dtos;

public class UserProfileUpdateRequestDto
{
    public required string GivenName { get; set; }
    public string? SurName { get; set; }
    public string? PhoneNumber { get; set; }
}