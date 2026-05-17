namespace FSP.Api.Application.Features.SpaceSports.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } = "reader";
        public bool CanPublish { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
