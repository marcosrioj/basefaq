using Querify.Models.Broadcast.Enums;
using Querify.Models.Common.Dtos;

namespace Querify.Models.Broadcast.Dtos.Thread;

public sealed class ThreadGetAllRequestDto : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? ChannelConnectionId { get; set; }
    public ThreadStatus? Status { get; set; }
}
