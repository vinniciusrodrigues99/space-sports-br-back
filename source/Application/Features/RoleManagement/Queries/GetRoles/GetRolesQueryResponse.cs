using FSP.Api.Application.Features.RoleManagement.Queries.GetRoles.DTOs;

namespace FSP.Api.Application.Features.RoleManagement.Queries.GetRoles
{
    public class GetRolesQueryResponse
    {
        public List<RoleDto> Roles { get; set; } = new();
        public string Message { get; set; } = "Roles obtidas com sucesso.";
    }
}
