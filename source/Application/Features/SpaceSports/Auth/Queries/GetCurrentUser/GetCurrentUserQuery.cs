using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Auth.Queries.GetCurrentUser
{
    public record GetCurrentUserQuery(Guid UserId) : IRequest<ResponseBase<UserDTO>>;
}
