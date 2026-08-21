using Querify.Models.Common.Dtos;
using Querify.Models.Tenant.Enums;

namespace Querify.Models.Tenant.Dtos.ChannelConnection;

public sealed class ChannelConnectionGetAllRequestDto : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? TenantId { get; set; }
    public ChannelConnectionKind? Kind { get; set; }
    public ChannelConnectionStatus? Status { get; set; }
    public bool? IsEnabled { get; set; }
}
