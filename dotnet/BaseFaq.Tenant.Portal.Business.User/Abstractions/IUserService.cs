using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.User.Dtos.User;

namespace BaseFaq.Tenant.Portal.Business.User.Abstractions;

public interface IUserService
{
    Task<UserDto> Create(UserCreateRequestDto requestDto, CancellationToken token);
    Task Delete(Guid id, CancellationToken token);
    Task<PagedResultDto<UserDto>> GetAll(UserGetAllRequestDto requestDto, CancellationToken token);
    Task<UserDto> GetById(Guid id, CancellationToken token);
    Task<UserDto> Update(Guid id, UserUpdateRequestDto requestDto, CancellationToken token);
}