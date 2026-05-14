

namespace FSP.Api.Application.Features.Autenticacao.Commands.Login.DTOs
{
    public class TokenResponseDTO
    {
        public required string Token { get; set; }
        public double ExpiraEm { get; set; }
        public string? RefreshToken { get; set; }
        public TokenInformationDTO InformacoesToken { get; set; } = new TokenInformationDTO();
    }

}
