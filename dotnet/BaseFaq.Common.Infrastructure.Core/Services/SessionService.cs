using BaseFaq.Common.Infrastructure.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public sealed class SessionService : ISessionService
{
    private const string TenantIdHeaderName = "X-Tenant-Id";
    private const string UserIdClaimType = "sub";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        EnsureSessionFromHttpContext();
    }

    public Guid? TenantId => EnsureSessionFromHttpContext()?.TenantId;

    public string? UserId => EnsureSessionFromHttpContext()?.UserId;

    public void Set(Guid? tenantId, string? userId)
    {
        SessionContext.Set(tenantId, userId);
    }

    public void Clear()
    {
        SessionContext.Clear();
    }

    public IDisposable Push(Guid? tenantId, string? userId)
    {
        return SessionContext.Push(tenantId, userId);
    }

    private SessionContext.ContextValues? EnsureSessionFromHttpContext()
    {
        if (SessionContext.OverrideValues is not null)
        {
            return SessionContext.OverrideValues;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return null;
        }

        var tenantValue = httpContext.Request.Headers.TryGetValue(TenantIdHeaderName, out var tenantHeader)
            ? tenantHeader.FirstOrDefault()
            : null;
        var tenantId = Guid.TryParse(tenantValue, out var parsedTenantId)
            ? parsedTenantId
            : (Guid?)null;

        var userId = httpContext.User?.FindFirst(UserIdClaimType)?.Value;

        SessionContext.Set(tenantId, userId);

        return SessionContext.OverrideValues;
    }
}

public static class SessionContext
{
    private static readonly AsyncLocal<ContextValues?> LocalValues = new();

    public static ContextValues? OverrideValues => LocalValues.Value;
    public static Guid? TenantId => LocalValues.Value?.TenantId;
    public static string? UserId => LocalValues.Value?.UserId;

    public static void Set(Guid? tenantId, string? userId)
    {
        LocalValues.Value = new ContextValues(tenantId, userId);
    }

    public static void Clear()
    {
        LocalValues.Value = null;
    }

    public static IDisposable Push(Guid? tenantId, string? userId)
    {
        var previous = LocalValues.Value;
        LocalValues.Value = new ContextValues(tenantId, userId);
        return new RevertScope(previous);
    }

    public sealed record ContextValues(Guid? TenantId, string? UserId);

    private sealed class RevertScope : IDisposable
    {
        private readonly ContextValues? _previous;
        private bool _disposed;

        public RevertScope(ContextValues? previous)
        {
            _previous = previous;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            LocalValues.Value = _previous;
            _disposed = true;
        }
    }
}