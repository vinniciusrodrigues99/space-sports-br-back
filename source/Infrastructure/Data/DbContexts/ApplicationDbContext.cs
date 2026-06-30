using System.Reflection;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Domain.Entities.Permission;
using FSP.Api.Domain.Entities.Post;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Constants;
using PermissionsConstants = FSP.Api.Domain.Constants.Permissions;
using FSP.Api.Domain.Interfaces.Data.Repositories;
using FSP.Api.Infrastructure.Data.Respositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using FSP.Api.Domain.Entities.Log;
using FSP.Api.Domain.Entities.Palpite;
using Microsoft.Extensions.Configuration;

namespace FSP.Api.Infrastructure.Data.DbContexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : IdentityDbContext<ApplicationUser, Perfil, Guid>(options), IApplicationDbContext, IUnitOfWork
{
    private readonly IConfiguration _configuration = configuration;
    private IDbContextTransaction? _currentTransaction;

    public DbSet<Permissao> Permissions { get; set; }
    public DbSet<Logs> Logs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Palpite> Palpites { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public int Commit() => base.SaveChanges();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await base.SaveChangesAsync(cancellationToken);

    public IRepositoryBase<TEntity> GetRepository<TEntity>() where TEntity : class
        => new RepositoryBase<TEntity>(this, _configuration);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await base.SaveChangesAsync(cancellationToken);
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public bool HasTransactionOpen() => _currentTransaction != null;
}
