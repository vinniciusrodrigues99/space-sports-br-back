using FSP.Api.Domain.Notifications;
using MediatR;

namespace FSP.Api.Application.Features.RoleManagement.Commands.AssignRoleToUser
{
    public class AssignRoleToUserCommand(Guid userId, AssignRoleToUserRequest assignRoleToUserRequest) : Command<AssignRoleToUserCommandResponse>
    {
        public Guid UserId { get; set; } = userId;
        public AssignRoleToUserRequest AssignRoleToUserRequest { get; set; } = assignRoleToUserRequest;
    }
}
