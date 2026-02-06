namespace BaseFaq.Common.EntityFramework.Core.Abstractions;

public interface ITenantConnectionStringProvider
{
    string GetConnectionString(Guid tenantId);
}