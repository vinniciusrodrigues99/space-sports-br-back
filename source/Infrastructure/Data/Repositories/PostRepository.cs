using FSP.Api.Domain.Entities.Post;
using FSP.Api.Domain.Enums;
using FSP.Api.Domain.Interfaces.Data.Repositories;
using FSP.Api.Infrastructure.Connector;
using FSP.Api.Infrastructure.Data.DbContexts;
using FSP.Api.Infrastructure.Data.Respositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FSP.Api.Infrastructure.Data.Repositories
{
    public class PostRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        : RepositoryBase<Post>(dbContext, configuration), IPostRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<List<Post>> GetAllComAutorAsync(CategoriaPost? categoria, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Posts
                .Include(p => p.Autor)
                .Where(p => !p.Excluido);

            if (categoria.HasValue)
                query = query.Where(p => p.Categoria == categoria.Value);

            return await query
                .OrderByDescending(p => p.PublicadoEm)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Post?> GetBySlugComAutorAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Posts
                .Include(p => p.Autor)
                .Where(p => p.Slug == slug && !p.Excluido)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Post?> GetByIdComAutorAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Posts
                .Include(p => p.Autor)
                .Where(p => p.Id == id && !p.Excluido)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(Post post, CancellationToken cancellationToken = default)
        {
            await _dbContext.Posts.AddAsync(post, cancellationToken);
        }
    }
}
