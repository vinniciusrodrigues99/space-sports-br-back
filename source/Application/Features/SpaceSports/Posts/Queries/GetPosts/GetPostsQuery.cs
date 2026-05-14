using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Queries.GetPosts
{
    public record GetPostsQuery(string? Category) : IRequest<ResponseBase<List<PostDTO>>>;
}
