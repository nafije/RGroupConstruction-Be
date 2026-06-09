using RGroupConstruction.Application.Interfaces;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Interfaces.Specification;
using RGroupConstruction.Domain.Common;
using RGroupConstruction.Domain.Enums;
using RGroupConstruction.Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RGroupConstruction.Infrastructure.Repositories;

public class Repository<T>(ApplicationDbContext context, IUserContext userContext) : IRepository<T> where T : class
{
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    private IQueryable<T> ActiveSet => typeof(IAuditedEntity).IsAssignableFrom(typeof(T))
        ? _dbSet.Where(e => !((IAuditedEntity)e).IsDeleted)
        : _dbSet;

    public virtual async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, "Id");
        var equals = Expression.Equal(property, Expression.Constant(id));
        var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

        return await ActiveSet.FirstOrDefaultAsync(lambda, cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync<TId>(TId id, Expression<Func<T, object>>[] includes, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = ActiveSet;
        query = includes.Aggregate(query, (current, include) => current.Include(include));

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, "Id");
        var equals = Expression.Equal(property, Expression.Constant(id));
        var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

        return await query.FirstOrDefaultAsync(lambda, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ActiveSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await ActiveSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await ActiveSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await ActiveSet.CountAsync(cancellationToken);

        return await ActiveSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await ActiveSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        ApplyAudit(entity, AuditAction.Added);
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var enumerable = entities as T[] ?? entities.ToArray();
        foreach (var entity in enumerable)
            ApplyAudit(entity, AuditAction.Added);

        await _dbSet.AddRangeAsync(enumerable, cancellationToken);
        return enumerable;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ApplyAudit(entity, AuditAction.Modified);
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var enumerable = entities as T[] ?? entities.ToArray();
        foreach (var entity in enumerable)
            ApplyAudit(entity, AuditAction.Modified);

        _dbSet.UpdateRange(enumerable);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (ApplyAudit(entity, AuditAction.Deleted))
            _dbSet.Update(entity);
        else
            _dbSet.Remove(entity);

        return Task.CompletedTask;
    }

    public virtual Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var enumerable = entities as T[] ?? entities.ToArray();
        var hardDeleteEntities = new List<T>();

        foreach (var entity in enumerable)
        {
            if (ApplyAudit(entity, AuditAction.Deleted))
                _dbSet.Update(entity);
            else
                hardDeleteEntities.Add(entity);
        }

        if (hardDeleteEntities.Count > 0)
            _dbSet.RemoveRange(hardDeleteEntities);

        return Task.CompletedTask;
    }

    public virtual IQueryable<T> AsQueryable()
    {
        return ActiveSet;
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(ActiveSet, spec);
    }

    private bool ApplyAudit(T entity, AuditAction action)
    {
        if (entity is not IAuditedEntity auditedEntity)
            return false;

        var now = DateTime.UtcNow;
        var userId = userContext.UserId;
        var ip = userContext.IpAddress;

        switch (action)
        {
            case AuditAction.Added:
                auditedEntity.CreatedOn = now;
                auditedEntity.CreatedBy = userId;
                auditedEntity.CreatedIP = ip;
                break;
            case AuditAction.Modified:
                auditedEntity.ModifiedOn = now;
                auditedEntity.ModifiedBy = userId;
                auditedEntity.ModifiedIP = ip;
                break;
            case AuditAction.Deleted:
                auditedEntity.IsDeleted = true;
                auditedEntity.DeletedOn = now;
                auditedEntity.DeletedBy = userId;
                auditedEntity.DeletedIP = ip;
                break;
        }

        return true;
    }
}

