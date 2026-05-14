namespace FSP.Api.Application.Features.Autenticacao.Commands.Login
{
    public record LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}
