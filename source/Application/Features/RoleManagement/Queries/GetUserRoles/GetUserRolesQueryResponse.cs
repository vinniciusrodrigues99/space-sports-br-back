using FSP.Api.Application.Features.RoleManagement.Queries.GetRoles.DTOs;

namespace FSP.Api.Application.Features.RoleManagement.Queries.GetUserRoles
{
    public class GetUserRolesQueryResponse
    {
        public Guid UserId { get; set; }
        public List<RoleDto> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
        public string Message { get; set; } = "Roles do usuário obtidas com sucesso.";
    }
}
