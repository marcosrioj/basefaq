using BaseFaq.Models.User.Dtos.User;
using MediatR;

namespace BaseFaq.Tenant.Portal.Business.User.Queries.GetUserProfile;

public class UsersGetUserProfileQuery : IRequest<UserProfileDto?>;