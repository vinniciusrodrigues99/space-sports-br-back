namespace FSP.Api.Application.Features.SpaceSports.DTOs
{
    public class AuthSessionDTO
    {
        public string Token { get; set; } = string.Empty;
        public UserDTO User { get; set; } = new();
    }
}
