using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Enums;
using FSP.Api.Domain.Interfaces.Data.Repositories;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Queries.GetPosts
{
    public class GetPostsQueryHandler(
        IMediator mediator,
        IPostRepository postRepository)
        : IRequestHandler<GetPostsQuery, ResponseBase<List<PostDTO>>>
    {
        public async Task<ResponseBase<List<PostDTO>>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
        {
            CategoriaPost? categoria = null;

            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                if (Enum.TryParse<CategoriaPost>(request.Category, true, out var cat))
                    categoria = cat;
                else
                {
                    await mediator.Publish(new DomainNotification("GetPosts", $"Categoria inválida: {request.Category}"), cancellationToken);
                    return ResponseBase<List<PostDTO>>.Failure();
                }
            }

            var posts = await postRepository.GetAllComAutorAsync(categoria, cancellationToken);

            var dtos = posts.Select(p => new PostDTO
            {
                Id = p.Id.ToString(),
                Title = p.Titulo,
                Slug = p.Slug,
                Excerpt = p.Resumo,
                Content = p.Conteudo,
                CoverUrl = p.CapaUrl,
                Category = p.Categoria.ToString().ToLower(),
                AuthorId = p.AutorId.ToString(),
                Author = new PostAuthorDTO
                {
                    Id = p.Autor?.Id.ToString() ?? p.AutorId.ToString(),
                    Name = p.Autor?.NomeCompleto ?? string.Empty,
                    AvatarUrl = p.Autor?.TxFoto
                },
                PublishedAt = p.PublicadoEm,
                ReadingMinutes = p.MinutosLeitura,
                ViewCount = p.Visualizacoes
            }).ToList();

            return ResponseBase<List<PostDTO>>.Success(dtos);
        }
    }
}
