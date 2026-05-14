
namespace FSP.Api.Application.Features.Autenticacao.Commands.RefreshToken
{
    public class RefreshTokenCommandResponse
    {
        public required string Token { get; set; }
        public double ExpiraEm { get; set; }
        public string? RefreshToken { get; set; }
    }
}
