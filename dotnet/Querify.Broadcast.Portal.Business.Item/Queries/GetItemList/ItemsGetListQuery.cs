using MediatR;
using Querify.Models.Broadcast.Dtos.Item;
using Querify.Models.Common.Dtos;

namespace Querify.Broadcast.Portal.Business.Item.Queries.GetItemList;

public sealed class ItemsGetListQuery : IRequest<PagedResultDto<ItemDto>>
{
    public required ItemGetAllRequestDto Request { get; set; }
}
