using MediatR;
using Querify.Models.Broadcast.Dtos.Thread;

namespace Querify.Broadcast.Portal.Business.Thread.Commands.CreateThread;

public sealed class ThreadsCreateCommand : IRequest<Guid>
{
    public required ThreadCreateRequestDto Request { get; set; }
}
