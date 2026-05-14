using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Users.Queries.GetAllUsers
{
    public record GetAllUsersQuery : IRequest<ResponseBase<List<UserDTO>>>;
}
