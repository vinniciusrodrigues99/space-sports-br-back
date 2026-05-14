using FSP.Api.Application.Features.Permission.Queries.GetPermissions.DTOs;

namespace FSP.Api.Application.Features.Permission.Queries.GetPermissions
{
    public class GetPermissionsQueryResponse
    {
        public List<PermissionDto> Permissions { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public string Message { get; set; } = "Permissões obtidas com sucesso.";
    }
}
