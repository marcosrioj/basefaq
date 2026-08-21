using MediatR;
using Querify.Broadcast.Portal.Business.Thread.Abstractions;
using Querify.Broadcast.Portal.Business.Thread.Commands.CreateThread;
using Querify.Broadcast.Portal.Business.Thread.Commands.DeleteThread;
using Querify.Broadcast.Portal.Business.Thread.Commands.UpdateThread;
using Querify.Broadcast.Portal.Business.Thread.Queries.GetThread;
using Querify.Broadcast.Portal.Business.Thread.Queries.GetThreadList;
using Querify.Models.Broadcast.Dtos.Thread;
using Querify.Models.Common.Dtos;

namespace Querify.Broadcast.Portal.Business.Thread.Service;

public sealed class ThreadService(IMediator mediator) : IThreadService
{
    public Task<Guid> Create(ThreadCreateRequestDto request, CancellationToken cancellationToken) =>
        mediator.Send(new ThreadsCreateCommand { Request = request }, cancellationToken);

    public Task<Guid> Update(Guid id, ThreadUpdateRequestDto request, CancellationToken cancellationToken) =>
        mediator.Send(new ThreadsUpdateCommand { Id = id, Request = request }, cancellationToken);

    public Task Delete(Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new ThreadsDeleteCommand { Id = id }, cancellationToken);

    public Task<ThreadDto> GetById(Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new ThreadsGetQuery { Id = id }, cancellationToken);

    public Task<PagedResultDto<ThreadDto>> GetAll(
        ThreadGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        mediator.Send(new ThreadsGetListQuery { Request = request }, cancellationToken);
}
