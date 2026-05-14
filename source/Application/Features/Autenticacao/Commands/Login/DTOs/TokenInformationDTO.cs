

namespace FSP.Api.Application.Features.Autenticacao.Commands.Login.DTOs
{
    public class TokenInformationDTO
    {
        public Guid Id { get; set; }
        public string NomeUsuario { get; set; } = String.Empty;
        public string? Email { get; set; }
        public IEnumerable<TokenClaimDTO> Claims { get; set; } = Enumerable.Empty<TokenClaimDTO>();
    }
}
