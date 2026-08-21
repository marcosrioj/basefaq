using Querify.Models.Broadcast.Dtos.Item;
using Querify.Models.Common.Dtos;

namespace Querify.Broadcast.Portal.Business.Item.Abstractions;

public interface IItemService
{
    Task<Guid> Create(ItemCreateRequestDto request, CancellationToken cancellationToken);
    Task<PagedResultDto<ItemDto>> GetAll(ItemGetAllRequestDto request, CancellationToken cancellationToken);
}
