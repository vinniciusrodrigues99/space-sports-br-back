namespace FSP.Api.Application.Features.RoleManagement.Commands.CreateRole
{
    public class CreateRoleCommandResponse
    {
        public string RoleName { get; set; } = string.Empty;
        public string Message { get; set; } = "Role criada com sucesso.";
    }
}
