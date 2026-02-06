namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface ISessionService
{
    Guid? TenantId { get; }
    void Set(Guid? tenantId, string? externalUserId);
    void Clear();
}