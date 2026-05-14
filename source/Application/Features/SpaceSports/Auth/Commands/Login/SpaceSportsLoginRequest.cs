namespace FSP.Api.Application.Features.SpaceSports.Auth.Commands.Login
{
    public class SpaceSportsLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
