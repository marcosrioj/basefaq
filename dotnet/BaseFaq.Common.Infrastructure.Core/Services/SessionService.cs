using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public sealed class SessionService : ISessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<SessionOptions> _options;

    public SessionService(
        IHttpContextAccessor httpContextAccessor,
        IOptions<SessionOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options;
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

        var tenantClaimType = _options.Value.TenantIdClaimType;
        var tenantValue = httpContext.User?.FindFirst(tenantClaimType)?.Value;
        var tenantId = Guid.TryParse(tenantValue, out var parsedTenantId)
            ? parsedTenantId
            : (Guid?)null;

        var userClaimType = _options.Value.UserIdClaimType;
        var userId = httpContext.User?.FindFirst(userClaimType)?.Value;

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