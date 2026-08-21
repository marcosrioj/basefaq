using MediatR;

namespace Querify.Broadcast.Portal.Business.Thread.Commands.DeleteThread;

public sealed class ThreadsDeleteCommand : IRequest
{
    public required Guid Id { get; set; }
}
