using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Users.Commands.UpdateUserRole
{
    public record UpdateUserRoleCommand(Guid UserId, UpdateUserRoleRequest Request) : IRequest<ResponseBase<UserDTO>>;
}
