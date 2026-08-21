using MediatR;
using Querify.Models.Broadcast.Dtos.Item;

namespace Querify.Broadcast.Portal.Business.Item.Commands.CreateItem;

public sealed class ItemsCreateCommand : IRequest<Guid>
{
    public required ItemCreateRequestDto Request { get; set; }
}
