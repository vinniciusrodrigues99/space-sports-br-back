using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Commands.UpdatePost
{
    public record UpdatePostCommand(Guid PostId, UpdatePostRequest Request, Guid RequestUserId, bool IsAdmin) : IRequest<ResponseBase<PostDTO>>;
}
