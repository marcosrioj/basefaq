namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface ISessionService
{
    Guid? TenantId { get; }
    string? ExternalUserId { get; }

    void Set(Guid? tenantId, string? externalUserId);
    void Clear();
}