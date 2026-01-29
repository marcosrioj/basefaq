namespace BaseFaq.Common.EntityFramework.Tenant.Repositories;

public interface ITenantRepository
{
    IQueryable<Entities.Tenant> Query(bool tracking = true);
    Task<List<Entities.Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Entities.Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Entities.Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.Tenant tenant, CancellationToken cancellationToken = default);
    void Update(Entities.Tenant tenant);
    void Remove(Entities.Tenant tenant, bool softDelete = true);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}