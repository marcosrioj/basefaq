using MediatR;
using Querify.Models.Broadcast.Dtos.Thread;
using Querify.Models.Common.Dtos;

namespace Querify.Broadcast.Portal.Business.Thread.Queries.GetThreadList;

public sealed class ThreadsGetListQuery : IRequest<PagedResultDto<ThreadDto>>
{
    public required ThreadGetAllRequestDto Request { get; set; }
}
