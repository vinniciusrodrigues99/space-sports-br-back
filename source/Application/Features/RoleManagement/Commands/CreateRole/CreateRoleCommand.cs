using FSP.Api.Application.Features.RoleManagement.Commands.CreateRole;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.RoleManagement.Commands.CreateRole
{
    public class CreateRoleCommand(CreateRoleRequest createRoleRequest) : Command<CreateRoleCommandResponse>
    {
        public CreateRoleRequest CreateRoleRequest { get; set; } = createRoleRequest;
    }
}
