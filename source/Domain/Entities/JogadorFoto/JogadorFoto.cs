using FSP.Api.Domain.Entities.BaseAuditable;

namespace FSP.Api.Domain.Entities.JogadorFoto
{
    public class JogadorFoto : BaseAuditableEntity
    {
        public required string PlayerId { get; set; }
        public required string PlayerName { get; set; }
        public required string NomeArquivo { get; set; }
        public Guid? AtualizadoPorUserId { get; set; }
    }
}
