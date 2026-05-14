using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Commands.DeletePost
{
    public record DeletePostCommand(Guid PostId, Guid RequestUserId, bool IsAdmin) : IRequest<ResponseBase<bool>>;
}
