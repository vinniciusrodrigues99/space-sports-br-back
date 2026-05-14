using FSP.Api.Domain.Entities.Log;
using FSP.Api.Domain.Entities.Permission;
using FSP.Api.Domain.Entities.Post;
using Microsoft.EntityFrameworkCore;

namespace FSP.Api.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Permissao> Permissions { get; }
    DbSet<Logs> Logs { get; }
    DbSet<Post> Posts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
