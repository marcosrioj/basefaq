namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface ISessionService
{
    Guid? TenantId { get; }
    string? UserId { get; }

    void Set(Guid? tenantId, string? userId);
    void Clear();
    IDisposable Push(Guid? tenantId, string? userId);
}