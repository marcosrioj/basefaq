using BaseFaq.Common.EntityFramework.Core.Repositories;
using BaseFaq.Common.EntityFramework.Core.Security;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Common.EntityFramework.Tenant.Repositories;

public sealed class TenantRepository
    : BaseRepository<TenantDbContext, Entities.Tenant>, ITenantRepository
{
    public TenantRepository(TenantDbContext context)
        : base(context)
    {
    }

    public override async Task<List<Entities.Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await Set.AsNoTracking().ToListAsync(cancellationToken);
        foreach (var tenant in tenants)
        {
            DecryptConnectionString(tenant);
        }

        return tenants;
    }

    public override async Task<Entities.Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await Set.AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

        return DecryptConnectionString(tenant);
    }

    public async Task<Entities.Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var tenant = await Set.AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Slug == slug, cancellationToken);

        return DecryptConnectionString(tenant);
    }

    public override Task AddAsync(Entities.Tenant tenant, CancellationToken cancellationToken = default)
    {
        EncryptConnectionString(tenant);
        return base.AddAsync(tenant, cancellationToken);
    }

    public override void Update(Entities.Tenant tenant)
    {
        EncryptConnectionString(tenant);
        base.Update(tenant);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EncryptTrackedTenants();
        return base.SaveChangesAsync(cancellationToken);
    }

    private static Entities.Tenant? DecryptConnectionString(Entities.Tenant? tenant)
    {
        if (tenant is null || !IsEncrypted(tenant.ConnectionString))
        {
            return tenant;
        }

        tenant.ConnectionString = StringCipher.Instance.Decrypt(tenant.ConnectionString) ?? string.Empty;
        return tenant;
    }

    private static void EncryptConnectionString(Entities.Tenant tenant)
    {
        if (IsEncrypted(tenant.ConnectionString))
        {
            return;
        }

        tenant.ConnectionString = StringCipher.Instance.Encrypt(tenant.ConnectionString) ?? string.Empty;
    }

    private void EncryptTrackedTenants()
    {
        foreach (var entry in Context.ChangeTracker.Entries<Entities.Tenant>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                EncryptConnectionString(entry.Entity);
            }
        }
    }

    private static bool IsEncrypted(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.StartsWith("v1:", StringComparison.Ordinal);
    }
}