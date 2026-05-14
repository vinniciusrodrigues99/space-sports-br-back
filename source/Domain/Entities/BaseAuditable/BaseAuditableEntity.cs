using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSP.Api.Domain.Entities.BaseAuditable
{
    public class BaseAuditableEntity : IBaseAuditableEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset CriadoEm { get; set; }
        public string? CriadoPor { get; set; }
        public DateTimeOffset DataModificacao { get; set; }
        public string? ModificadoPor { get; set; }
        public bool Excluido { get; set; } = false;
    }
    
    public interface IBaseAuditableEntity 
    {
        public Guid Id { get; set; }
        public DateTimeOffset CriadoEm { get; set; }
        public string? CriadoPor { get; set; }
        public DateTimeOffset DataModificacao { get; set; }
        public string? ModificadoPor { get; set; }
        public bool Excluido { get; set; }
    }
}