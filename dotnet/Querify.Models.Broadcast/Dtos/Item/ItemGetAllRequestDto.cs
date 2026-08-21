using Querify.Models.Broadcast.Enums;
using Querify.Models.Common.Dtos;

namespace Querify.Models.Broadcast.Dtos.Item;

public sealed class ItemGetAllRequestDto : PagedAndSortedResultRequestDto
{
    public required Guid ThreadId { get; set; }
    public ItemKind? Kind { get; set; }
    public ActorKind? ActorKind { get; set; }
}
