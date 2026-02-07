using System.Linq.Expressions;
using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Core.Entities;
using BaseFaq.Common.EntityFramework.Core.Helpers;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Models.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace BaseFaq.Common.EntityFramework.Core;

public abstract class BaseDbContext<TContext> : DbContext where TContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly ISessionService _sessionService;
    private readonly ITenantConnectionStringProvider _tenantConnectionStringProvider;

    protected BaseDbContext(
        DbContextOptions<TContext> options,
        ISessionService sessionService,
        IConfiguration configuration,
        ITenantConnectionStringProvider tenantConnectionStringProvider)
        : base(options)
    {
        _sessionService = sessionService;
        _configuration = configuration;
        _tenantConnectionStringProvider = tenantConnectionStringProvider;
    }

    protected virtual IEnumerable<string> ConfigurationNamespaces => [];

    protected virtual bool UseTenantConnectionString => true;

    protected abstract AppEnum SessionApp { get; }

    protected Guid? SessionTenantId => UseTenantConnectionString ? _sessionService.GetTenantId(SessionApp) : null;

    public bool GlobalFiltersEnabled { get; set; } = true;

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplySoftDeleteRules();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplySoftDeleteRules();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplySoftDeleteRules();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseCollation("Latin1_General_100_CI_AS_SC_UTF8");

        var configurationLoader = new EfConfigurationLoader<TContext>();
        foreach (var configurationNamespace in ConfigurationNamespaces)
        {
            configurationLoader.LoadFromNameSpace(modelBuilder, configurationNamespace);
        }

        ApplyGlobalFilters(modelBuilder);
        ApplyTenantIndexes(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var connectionString = ResolveConnectionString();

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    private string GetDefaultConnectionString()
    {
        const string defaultConnectionStringName = "DefaultConnection";
        var defaultConnectionString = _configuration.GetConnectionString(defaultConnectionStringName);

        if (string.IsNullOrWhiteSpace(defaultConnectionString))
        {
            throw new ApiErrorException(
                $"Missing connection string '{defaultConnectionStringName}'.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return defaultConnectionString;
    }

    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var filter = BuildFilterExpression(entityType.ClrType);
            if (filter is null)
            {
                continue;
            }

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }
    }

    private LambdaExpression? BuildFilterExpression(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        Expression? filterBody = null;

        if (typeof(ISoftDelete).IsAssignableFrom(entityType))
        {
            var isDeletedProperty = Expression.Property(
                Expression.Convert(parameter, typeof(ISoftDelete)),
                nameof(ISoftDelete.IsDeleted));

            var notDeleted = Expression.Not(isDeletedProperty);
            filterBody = filterBody is null ? notDeleted : Expression.AndAlso(filterBody, notDeleted);
        }

        var currentTenantId = Expression.Property(Expression.Constant(this), nameof(SessionTenantId));
        var tenantIsNull = Expression.Equal(currentTenantId, Expression.Constant(null, typeof(Guid?)));

        if (typeof(IMustHaveTenant).IsAssignableFrom(entityType))
        {
            var tenantProperty = Expression.Property(
                Expression.Convert(parameter, typeof(IMustHaveTenant)),
                nameof(IMustHaveTenant.TenantId));

            var tenantMatches = Expression.Equal(
                Expression.Convert(tenantProperty, typeof(Guid?)),
                currentTenantId);

            var tenantFilter = Expression.OrElse(tenantIsNull, tenantMatches);
            filterBody = filterBody is null ? tenantFilter : Expression.AndAlso(filterBody, tenantFilter);
        }
        else if (typeof(IMayHaveTenant).IsAssignableFrom(entityType))
        {
            var tenantProperty = Expression.Property(
                Expression.Convert(parameter, typeof(IMayHaveTenant)),
                nameof(IMayHaveTenant.TenantId));

            var tenantIsNullOnEntity = Expression.Equal(
                tenantProperty,
                Expression.Constant(null, typeof(Guid?)));

            var tenantMatches = Expression.Equal(tenantProperty, currentTenantId);
            var tenantFilter = Expression.OrElse(
                tenantIsNull,
                Expression.OrElse(tenantIsNullOnEntity, tenantMatches));

            filterBody = filterBody is null ? tenantFilter : Expression.AndAlso(filterBody, tenantFilter);
        }

        if (filterBody is null)
        {
            return null;
        }

        var globalFiltersEnabled = Expression.Property(
            Expression.Constant(this),
            nameof(GlobalFiltersEnabled));

        var ignoreFilters = Expression.Not(globalFiltersEnabled);
        var finalFilter = Expression.OrElse(ignoreFilters, filterBody);

        return Expression.Lambda(finalFilter, parameter);
    }

    private void ApplySoftDeleteRules()
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;

            if (entry.Entity is AuditableEntity auditableEntity)
            {
                auditableEntity.DeletedDate = DateTime.UtcNow;
            }
        }
    }

    private static void ApplyTenantIndexes(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IMustHaveTenant).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex(nameof(IMustHaveTenant.TenantId))
                    .HasDatabaseName($"IX_{entityType.ClrType.Name}_TenantId");
                continue;
            }

            if (typeof(IMayHaveTenant).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex(nameof(IMayHaveTenant.TenantId))
                    .HasDatabaseName($"IX_{entityType.ClrType.Name}_TenantId");
            }
        }
    }

    private sealed class ResetOnDispose : IDisposable
    {
        private readonly Action _reset;
        private bool _disposed;

        public ResetOnDispose(Action reset)
        {
            _reset = reset;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _reset();
            _disposed = true;
        }
    }

    private string ResolveConnectionString()
    {
        if (!UseTenantConnectionString)
        {
            return GetDefaultConnectionString();
        }

        var tenantId = _sessionService.GetTenantId(SessionApp);

        var tenantConnectionString = _tenantConnectionStringProvider.GetConnectionString(tenantId);

        if (string.IsNullOrWhiteSpace(tenantConnectionString))
        {
            throw new ApiErrorException(
                $"Tenant '{tenantId}' has an invalid connection string.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return tenantConnectionString;
    }
}