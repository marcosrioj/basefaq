using BaseFaq.Models.Tenant.Enums;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Commands.CreateUser;

public class UsersCreateUserCommand : IRequest<Guid>
{
    public required string GivenName { get; set; }
    public string? SurName { get; set; }
    public required string Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? PhoneNumber { get; set; }
    public required UserRoleType Role { get; set; }
    public required Guid TenantId { get; set; }
}