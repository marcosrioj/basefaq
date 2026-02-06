namespace BaseFaq.Models.Tenant.Dtos.TenantConnection;

public class TenantConnectionCreateRequestDto
{
    public required Guid TenantId { get; set; }
    public required string ConnectionString { get; set; }
    public required bool IsCurrent { get; set; }
}