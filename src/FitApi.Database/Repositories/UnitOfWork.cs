using FitApi.Core.Repositories;

namespace FitApi.Database.Repositories;

public class UnitOfWork(FitDbContext dbContext) : IUnitOfWork
{
    private bool HasActiveTransaction => dbContext.Database.CurrentTransaction != null;

    public Task BeginAsync(CancellationToken cancellationToken)
    {
        return dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return HasActiveTransaction ? dbContext.Database.CommitTransactionAsync(cancellationToken) : Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken cancellationToken)
    {
        return HasActiveTransaction
            ? dbContext.Database.RollbackTransactionAsync(cancellationToken)
            : Task.CompletedTask;
        ;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}