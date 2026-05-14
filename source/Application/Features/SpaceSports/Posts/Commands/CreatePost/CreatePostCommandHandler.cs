using FSP.Api.Application.Common.Helpers;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Post;
using FSP.Api.Domain.Enums;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Interfaces.Data.Repositories;
using FSP.Api.Domain.Notifications;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Commands.CreatePost
{
    public class CreatePostCommandHandler(
        IMediator mediator,
        IPostRepository postRepository,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ILogService logService)
        : IRequestHandler<CreatePostCommand, ResponseBase<PostDTO>>
    {
        public async Task<ResponseBase<PostDTO>> Handle(CreatePostCommand command, CancellationToken cancellationToken)
        {
            var req = command.Request;

            if (!Enum.TryParse<CategoriaPost>(req.Category, true, out var categoria))
            {
                await mediator.Publish(new DomainNotification("CreatePost", $"Categoria inválida: {req.Category}"), cancellationToken);
                return ResponseBase<PostDTO>.Failure();
            }

            var autor = await userManager.FindByIdAsync(command.AutorId.ToString());
            if (autor == null)
            {
                await mediator.Publish(new DomainNotification("CreatePost", "Autor não encontrado."), cancellationToken);
                return ResponseBase<PostDTO>.Failure();
            }

            var slugBase = SlugHelper.Generate(req.Title);
            var slug = await ResolverSlugUnicoAsync(slugBase, cancellationToken);
            var minutosLeitura = SlugHelper.CalcularMinutosLeitura(req.Content);

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Titulo = req.Title,
                Slug = slug,
                Resumo = req.Excerpt,
                Conteudo = req.Content,
                CapaUrl = req.CoverUrl,
                Categoria = categoria,
                AutorId = command.AutorId,
                PublicadoEm = DateTimeOffset.UtcNow,
                MinutosLeitura = minutosLeitura,
                CriadoPor = autor.NomeCompleto,
                CriadoEm = DateTimeOffset.UtcNow,
                DataModificacao = DateTimeOffset.UtcNow
            };

            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                await postRepository.AddAsync(post, cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);

                await logService.Information(
                    descricao: $"Post criado: {post.Titulo}",
                    detalhes: System.Text.Json.JsonSerializer.Serialize(new
                    {
                        PostId = post.Id,
                        AutorId = post.AutorId,
                        Slug = post.Slug,
                        Acao = "CriarPost",
                        Modulo = "Posts"
                    })
                );

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
                        Id = autor.Id.ToString(),
                        Name = autor.NomeCompleto,
                        AvatarUrl = autor.TxFoto
                    },
                    PublishedAt = post.PublicadoEm,
                    ReadingMinutes = post.MinutosLeitura
                };

                return ResponseBase<PostDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                if (unitOfWork.HasTransactionOpen())
                    await unitOfWork.RollbackAsync(cancellationToken);

                await logService.Error(
                    descricao: "Erro ao criar post",
                    detalhes: ex.Message,
                    exception: ex
                );

                await mediator.Publish(new DomainNotification("CreatePost", "Erro ao criar o post."), cancellationToken);
                return ResponseBase<PostDTO>.Failure();
            }
        }

        private async Task<string> ResolverSlugUnicoAsync(string slugBase, CancellationToken cancellationToken)
        {
            var slug = slugBase;
            var existente = await postRepository.GetBySlugComAutorAsync(slug, cancellationToken);
            if (existente == null)
                return slug;

            var sufixo = 1;
            do
            {
                slug = $"{slugBase}-{sufixo++}";
                existente = await postRepository.GetBySlugComAutorAsync(slug, cancellationToken);
            } while (existente != null);

            return slug;
        }
    }
}
