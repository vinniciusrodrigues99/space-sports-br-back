using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Posts.Commands.IncrementPostView
{
    public record IncrementPostViewCommand(string Slug) : IRequest<ResponseBase<bool>>;
}
