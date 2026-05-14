using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Interfaces.Data.Repositories;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Commands.DeletePost
{
    public class DeletePostCommandHandler(
        IMediator mediator,
        IPostRepository postRepository,
        IUnitOfWork unitOfWork,
        ILogService logService)
        : IRequestHandler<DeletePostCommand, ResponseBase<bool>>
    {
        public async Task<ResponseBase<bool>> Handle(DeletePostCommand command, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdComAutorAsync(command.PostId, cancellationToken);

            if (post == null)
            {
                await mediator.Publish(new DomainNotification("DeletePost", "Post não encontrado."), cancellationToken);
                return ResponseBase<bool>.Failure();
            }

            if (!command.IsAdmin && post.AutorId != command.RequestUserId)
            {
                await mediator.Publish(new DomainNotification("DeletePost", "Sem permissão para excluir este post."), cancellationToken);
                return ResponseBase<bool>.Failure();
            }

            post.Excluido = true;
            post.DataModificacao = DateTimeOffset.UtcNow;

            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                postRepository.Update(post);
                await unitOfWork.CommitAsync(cancellationToken);

                await logService.Information(
                    descricao: $"Post excluído: {post.Titulo}",
                    detalhes: System.Text.Json.JsonSerializer.Serialize(new
                    {
                        PostId = post.Id,
                        Acao = "ExcluirPost",
                        Modulo = "Posts"
                    })
                );

                return ResponseBase<bool>.Success(true);
            }
            catch (Exception ex)
            {
                if (unitOfWork.HasTransactionOpen())
                    await unitOfWork.RollbackAsync(cancellationToken);

                await logService.Error("Erro ao excluir post", ex.Message, ex);
                await mediator.Publish(new DomainNotification("DeletePost", "Erro ao excluir o post."), cancellationToken);
                return ResponseBase<bool>.Failure();
            }
        }
    }
}
