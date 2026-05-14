namespace FSP.Api.Application.Features.RoleManagement.Queries.GetRoles.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? NormalizedName { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
