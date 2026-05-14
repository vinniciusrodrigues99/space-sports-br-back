namespace FSP.Api.Application.Features.RoleManagement.Commands.CreateRole
{
    public class CreateRoleRequest
    {
        public required string RoleName { get; set; }
        public string? Description { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
