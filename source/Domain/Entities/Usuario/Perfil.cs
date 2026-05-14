using FSP.Api.Domain.Entities.BaseAuditable;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Domain.Entities.Usuario
{
    public class Perfil : IdentityRole<Guid>, IBaseAuditableEntity
    {
        public string? CriadoPor { get; set; }
        public DateTimeOffset CriadoEm { get; set; }
        public string? ModificadoPor { get; set; }
        public DateTimeOffset DataModificacao { get; set; }
        public bool Excluido { get; set; } = false;
    }
}
