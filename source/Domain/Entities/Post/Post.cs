using FSP.Api.Domain.Entities.BaseAuditable;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Enums;

namespace FSP.Api.Domain.Entities.Post
{
    public class Post : BaseAuditableEntity
    {
        public required string Titulo { get; set; }
        public required string Slug { get; set; }
        public required string Resumo { get; set; }
        public required string Conteudo { get; set; }
        public string? CapaUrl { get; set; }
        public CategoriaPost Categoria { get; set; }
        public Guid AutorId { get; set; }
        public ApplicationUser? Autor { get; set; }
        public DateTimeOffset PublicadoEm { get; set; }
        public int MinutosLeitura { get; set; }
        public int Visualizacoes { get; set; }
    }
}
