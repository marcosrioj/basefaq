using MediatR;
using Querify.Models.Broadcast.Dtos.Thread;

namespace Querify.Broadcast.Portal.Business.Thread.Commands.UpdateThread;

public sealed class ThreadsUpdateCommand : IRequest<Guid>
{
    public required Guid Id { get; set; }
    public required ThreadUpdateRequestDto Request { get; set; }
}
