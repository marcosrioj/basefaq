using Querify.Models.Broadcast.Dtos.Thread;
using Querify.Models.Common.Dtos;

namespace Querify.Broadcast.Portal.Business.Thread.Abstractions;

public interface IThreadService
{
    Task<Guid> Create(ThreadCreateRequestDto request, CancellationToken cancellationToken);
    Task<Guid> Update(Guid id, ThreadUpdateRequestDto request, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
    Task<ThreadDto> GetById(Guid id, CancellationToken cancellationToken);
    Task<PagedResultDto<ThreadDto>> GetAll(ThreadGetAllRequestDto request, CancellationToken cancellationToken);
}
