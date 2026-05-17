using FSP.Api.Domain.Common;
using FSP.Api.Domain.Interfaces.Data.Repositories;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Commands.IncrementPostView
{
    public class IncrementPostViewCommandHandler(
        IMediator mediator,
        IPostRepository postRepository)
        : IRequestHandler<IncrementPostViewCommand, ResponseBase<bool>>
    {
        public async Task<ResponseBase<bool>> Handle(IncrementPostViewCommand request, CancellationToken cancellationToken)
        {
            var success = await postRepository.IncrementViewAsync(request.Slug, cancellationToken);

            if (!success)
            {
                await mediator.Publish(new DomainNotification("IncrementPostView", "Post não encontrado."), cancellationToken);
                return ResponseBase<bool>.Failure();
            }

            return ResponseBase<bool>.Success(true);
        }
    }
}
