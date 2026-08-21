using Querify.Models.Tenant.Enums;

namespace Querify.Models.Tenant.Dtos.ChannelConnection;

public sealed class ChannelConnectionCreateRequestDto
{
    public required string Name { get; set; }
    public required string ProviderKey { get; set; }
    public required ChannelConnectionKind Kind { get; set; }
    public required string ConnectionData { get; set; }
    public required bool IsEnabled { get; set; }
}
