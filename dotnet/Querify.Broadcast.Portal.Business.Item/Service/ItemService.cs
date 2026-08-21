using MediatR;
using Querify.Broadcast.Portal.Business.Item.Abstractions;
using Querify.Broadcast.Portal.Business.Item.Commands.CreateItem;
using Querify.Broadcast.Portal.Business.Item.Queries.GetItemList;
using Querify.Models.Broadcast.Dtos.Item;
using Querify.Models.Common.Dtos;

namespace Querify.Broadcast.Portal.Business.Item.Service;

public sealed class ItemService(IMediator mediator) : IItemService
{
    public Task<Guid> Create(ItemCreateRequestDto request, CancellationToken cancellationToken) =>
        mediator.Send(new ItemsCreateCommand { Request = request }, cancellationToken);

    public Task<PagedResultDto<ItemDto>> GetAll(
        ItemGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        mediator.Send(new ItemsGetListQuery { Request = request }, cancellationToken);
}
