using FSP.Api.Domain.Entities.BaseAuditable;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Domain.Entities.Usuario
{
    public class ApplicationUser : IdentityUser<Guid>, IBaseAuditableEntity
    {
        public required string NomeCompleto { get; set; }
        public string? CPF { get; set; }
        public string? TxFoto { get; set; }
        public string? Telefone { get; set; }
        public bool NotificacaoEmail { get; set; } = true;
        public bool NotificacaoSMS { get; set; } = false;
        public bool CanPublish { get; set; } = false;
        public string? RefreshToken { get; set; }
        public DateTime? DataExpiracaoRefreshToken { get; set; }
        public string? PrimeiroTokenAcesso { get; set; }
        public DateTime? ExpiracaoPrimeiroAcesso { get; set; }
        public string? CriadoPor { get; set; }
        public DateTimeOffset CriadoEm { get; set; }
        public string? ModificadoPor { get; set; }
        public DateTimeOffset DataModificacao { get; set; }
        public bool Excluido { get; set; } = false;
    }
}
