using FSP.Api.Domain.Interfaces.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FSP.Api.Application.Common.Interfaces;
public interface IUnitOfWork
{
    int Commit();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    IRepositoryBase<TEntity> GetRepository<TEntity>() where TEntity : class;
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    bool HasTransactionOpen();
}
