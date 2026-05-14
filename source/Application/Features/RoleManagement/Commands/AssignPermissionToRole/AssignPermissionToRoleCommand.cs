using FSP.Api.Application.Features.RoleManagement.Commands.AssignPermissionToRole;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.RoleManagement.Commands.AssignPermissionToRole
{
    public class AssignPermissionToRoleCommand(string roleName, AssignPermissionToRoleRequest assignPermissionToRoleRequest) : Command<AssignPermissionToRoleCommandResponse>
    {
        public string RoleName { get; set; } = roleName;
        public AssignPermissionToRoleRequest AssignPermissionToRoleRequest { get; set; } = assignPermissionToRoleRequest;
    }
}
