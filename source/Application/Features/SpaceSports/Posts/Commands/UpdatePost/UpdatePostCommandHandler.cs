using FSP.Api.Application.Common.Helpers;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Enums;
using FSP.Api.Domain.Interfaces.Data.Repositories;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Commands.UpdatePost
{
    public class UpdatePostCommandHandler(
        IMediator mediator,
        IPostRepository postRepository,
        IUnitOfWork unitOfWork,
        ILogService logService)
        : IRequestHandler<UpdatePostCommand, ResponseBase<PostDTO>>
    {
        public async Task<ResponseBase<PostDTO>> Handle(UpdatePostCommand command, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdComAutorAsync(command.PostId, cancellationToken);

            if (post == null)
            {
                await mediator.Publish(new DomainNotification("UpdatePost", "Post não encontrado."), cancellationToken);
                return ResponseBase<PostDTO>.Failure();
            }

            if (!command.IsAdmin && post.AutorId != command.RequestUserId)
            {
                await mediator.Publish(new DomainNotification("UpdatePost", "Sem permissão para editar este post."), cancellationToken);
                return ResponseBase<PostDTO>.Failure();
            }

            if (!Enum.TryParse<CategoriaPost>(command.Request.Category, true, out var categoria))
            {
                await mediator.Publish(new DomainNotification("UpdatePost", $"Categoria inválida: {command.Request.Category}"), cancellationToken);
                return ResponseBase<PostDTO>.Failure();
            }

            post.Titulo = command.Request.Title;
            post.Resumo = command.Request.Excerpt;
            post.Conteudo = command.Request.Content;
            post.CapaUrl = command.Request.CoverUrl;
            post.Categoria = categoria;
            post.MinutosLeitura = SlugHelper.CalcularMinutosLeitura(command.Request.Content);
            post.DataModificacao = DateTimeOffset.UtcNow;

            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                postRepository.Update(post);
                await unitOfWork.CommitAsync(cancellationToken);

                await logService.Information(
                    descricao: $"Post atualizado: {post.Titulo}",
                    detalhes: System.Text.Json.JsonSerializer.Serialize(new
                    {
                        PostId = post.Id,
                        Acao = "AtualizarPost",
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
                        Id = post.Autor?.Id.ToString() ?? post.AutorId.ToString(),
                        Name = post.Autor?.NomeCompleto ?? string.Empty,
                        AvatarUrl = post.Autor?.TxFoto
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

                await logService.Error("Erro ao atualizar post", ex.Message, ex);
                await mediator.Publish(new DomainNotification("UpdatePost", "Erro ao atualizar o post."), cancellationToken);
                return ResponseBase<PostDTO>.Failure();
            }
        }
    }
}
