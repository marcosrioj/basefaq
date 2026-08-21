using MediatR;
using Querify.Models.Broadcast.Dtos.Thread;

namespace Querify.Broadcast.Portal.Business.Thread.Queries.GetThread;

public sealed class ThreadsGetQuery : IRequest<ThreadDto>
{
    public required Guid Id { get; set; }
}
