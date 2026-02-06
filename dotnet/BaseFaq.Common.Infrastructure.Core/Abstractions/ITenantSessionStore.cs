namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface ITenantSessionStore
{
    Guid? GetTenantId(string externalUserId);
    void SetTenantId(string externalUserId, Guid tenantId);
    void RemoveTenantId(string externalUserId);
}