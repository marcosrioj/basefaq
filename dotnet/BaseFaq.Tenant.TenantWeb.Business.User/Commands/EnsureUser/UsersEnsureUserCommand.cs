using BaseFaq.Common.EntityFramework.Tenant.Entities;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.User.Commands.EnsureUser;

public class UsersEnsureUserCommand : IRequest<Common.EntityFramework.Tenant.Entities.User>
{
}