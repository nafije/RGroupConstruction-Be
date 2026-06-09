using RGroupConstruction.Application.Interfaces;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Infrastructure.Presistence.UnitOfWork;


public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;



    public UnitOfWork(
        ApplicationDbContext context,
        IUserContext userContext,
        ILogger<UnitOfWork> logger)
    {
        _context = context;
        _userContext = userContext;
        _logger = logger;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>).MakeGenericType(type);
            var repositoryInstance = Activator.CreateInstance(repositoryType, _context, _userContext);
            _repositories.Add(type, repositoryInstance!);
        }
        return (IRepository<T>)_repositories[type];
    }


    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Debug("Saving changes to database");
            var count = await _context.SaveChangesAsync(cancellationToken);
            _logger.Debug("Saved {ChangeCount} change(s) to database", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to save changes to database");
            throw;
        }
    }

    public void Dispose() => _context.Dispose();

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _logger.Debug("Started database transaction {TransactionId}", _transaction.TransactionId);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                _logger.Debug("Committed database transaction {TransactionId}", _transaction.TransactionId);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to commit database transaction");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            _logger.Warn("Rolled back database transaction {TransactionId}", _transaction.TransactionId);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}

