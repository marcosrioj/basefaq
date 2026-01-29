using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Common.EntityFramework.Core.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> Query(bool tracking = true);
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity, bool softDelete = true);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class BaseRepository<TContext, TEntity> : IBaseRepository<TEntity>
    where TContext : DbContext
    where TEntity : BaseEntity
{
    protected BaseRepository(TContext context)
    {
        Context = context;
        Set = context.Set<TEntity>();
    }

    protected TContext Context { get; }
    protected DbSet<TEntity> Set { get; }

    public virtual IQueryable<TEntity> Query(bool tracking = true)
    {
        return tracking ? Set : Set.AsNoTracking();
    }

    public virtual Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Set.ToListAsync(cancellationToken);
    }

    public virtual Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Set.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return Set.AddAsync(entity, cancellationToken).AsTask();
    }

    public virtual void Update(TEntity entity)
    {
        Set.Update(entity);
    }

    public virtual void Remove(TEntity entity, bool softDelete = true)
    {
        if (softDelete && entity is ISoftDelete softDeleteEntity)
        {
            softDeleteEntity.IsDeleted = true;
            if (entity is AuditableEntity auditableEntity)
            {
                auditableEntity.DeletedDate = DateTime.UtcNow;
            }

            Set.Update(entity);
            return;
        }

        Set.Remove(entity);
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }
}