using FSP.Api.Domain.Entities.Post;
using FSP.Api.Domain.Enums;

namespace FSP.Api.Domain.Interfaces.Data.Repositories
{
    public interface IPostRepository : IRepositoryBase<Post>
    {
        Task<List<Post>> GetAllComAutorAsync(CategoriaPost? categoria, CancellationToken cancellationToken = default);
        Task<Post?> GetBySlugComAutorAsync(string slug, CancellationToken cancellationToken = default);
        Task<Post?> GetByIdComAutorAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Post post, CancellationToken cancellationToken = default);
    }
}
