namespace FSP.Api.Application.Features.Autenticacao.Commands.RefreshToken
{
    public record RefreshTokenRequest
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
