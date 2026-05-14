using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Queries.GetPostBySlug
{
    public record GetPostBySlugQuery(string Slug) : IRequest<ResponseBase<PostDTO>>;
}
