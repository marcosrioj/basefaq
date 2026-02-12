using MediatR;

namespace BaseFaq.Tenant.Portal.Business.User.Commands.UpdateUserProfile;

public class UsersUpdateUserProfileCommand : IRequest
{
    public required string GivenName { get; set; }
    public string? SurName { get; set; }
    public string? PhoneNumber { get; set; }
}