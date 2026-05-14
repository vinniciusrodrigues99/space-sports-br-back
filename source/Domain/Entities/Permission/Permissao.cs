using FSP.Api.Domain.Entities.BaseAuditable;

namespace FSP.Api.Domain.Entities.Permission
{
    public class Permissao : BaseAuditableEntity
    {
        public required string Nome { get; set; }
        public required string NomeExibicao { get; set; }
        public string? Descricao { get; set; }
        public string? Categoria { get; set; }
    }
}
