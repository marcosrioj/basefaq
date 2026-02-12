using BaseFaq.Tenant.Portal.Business.User.Dtos;
using MediatR;

namespace BaseFaq.Tenant.Portal.Business.User.Queries.GetUserProfile;

public class UsersGetUserProfileQuery : IRequest<UserProfileDto?>;