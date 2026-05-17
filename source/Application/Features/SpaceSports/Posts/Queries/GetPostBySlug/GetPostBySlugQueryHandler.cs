using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Interfaces.Data.Repositories;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Queries.GetPostBySlug
{
    public class GetPostBySlugQueryHandler(
        IMediator mediator,
        IPostRepository postRepository)
        : IRequestHandler<GetPostBySlugQuery, ResponseBase<PostDTO>>
    {
        public async Task<ResponseBase<PostDTO>> Handle(GetPostBySlugQuery request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetBySlugComAutorAsync(request.Slug, cancellationToken);

            if (post == null)
            {
                await mediator.Publish(new DomainNotification("GetPostBySlug", "Post não encontrado."), cancellationToken);
                return ResponseBase<PostDTO>.Failure();
            }

            var dto = new PostDTO
            {
                Id = post.Id.ToString(),
                Title = post.Titulo,
                Slug = post.Slug,
                Excerpt = post.Resumo,
                Content = post.Conteudo,
                CoverUrl = post.CapaUrl,
                Category = post.Categoria.ToString().ToLower(),
                AuthorId = post.AutorId.ToString(),
                Author = new PostAuthorDTO
                {
                    Id = post.Autor?.Id.ToString() ?? post.AutorId.ToString(),
                    Name = post.Autor?.NomeCompleto ?? string.Empty,
                    AvatarUrl = post.Autor?.TxFoto
                },
                PublishedAt = post.PublicadoEm,
                ReadingMinutes = post.MinutosLeitura,
                ViewCount = post.Visualizacoes
            };

            return ResponseBase<PostDTO>.Success(dto);
        }
    }
}
