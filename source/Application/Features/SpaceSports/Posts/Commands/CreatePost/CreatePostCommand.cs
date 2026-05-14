using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Commands.CreatePost
{
    public record CreatePostCommand(CreatePostRequest Request, Guid AutorId) : IRequest<ResponseBase<PostDTO>>;
}
