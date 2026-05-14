using FSP.Api.Domain.Entities.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Post
{
    public class PostConfiguration : IEntityTypeConfiguration<Domain.Entities.Post.Post>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Post.Post> builder)
        {
            builder.ToTable("T_POST");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnName("ID_POST");

            builder.Property(p => p.Titulo)
                .HasColumnName("TX_TITULO")
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(p => p.Slug)
                .HasColumnName("TX_SLUG")
                .IsRequired()
                .HasMaxLength(350);

            builder.Property(p => p.Resumo)
                .HasColumnName("TX_RESUMO")
                .IsRequired()
                .HasMaxLength(280);

            builder.Property(p => p.Conteudo)
                .HasColumnName("TX_CONTEUDO")
                .IsRequired();

            builder.Property(p => p.CapaUrl)
                .HasColumnName("TX_CAPA_URL")
                .HasMaxLength(1000);

            builder.Property(p => p.Categoria)
                .HasColumnName("TX_CATEGORIA")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.AutorId)
                .HasColumnName("AUTOR_ID")
                .IsRequired();

            builder.Property(p => p.PublicadoEm)
                .HasColumnName("DT_PUBLICADO_EM")
                .IsRequired();

            builder.Property(p => p.MinutosLeitura)
                .HasColumnName("QTD_MINUTOS_LEITURA")
                .IsRequired();

            builder.Property(p => p.CriadoPor)
                .HasColumnName("TX_CRIADO_POR")
                .HasMaxLength(100);

            builder.Property(p => p.CriadoEm)
                .HasColumnName("DT_CRIADO_EM")
                .IsRequired();

            builder.Property(p => p.ModificadoPor)
                .HasColumnName("TX_MODIFICADO_POR")
                .HasMaxLength(100);

            builder.Property(p => p.DataModificacao)
                .HasColumnName("DT_MODIFICACAO");

            builder.Property(p => p.Excluido)
                .HasColumnName("FL_EXCLUIDO")
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(p => p.Slug)
                .IsUnique()
                .HasDatabaseName("IX_POST_SLUG");

            builder.HasIndex(p => p.Categoria)
                .HasDatabaseName("IX_POST_CATEGORIA");

            builder.HasIndex(p => p.PublicadoEm)
                .HasDatabaseName("IX_POST_PUBLICADO_EM");

            builder.HasIndex(p => p.Excluido)
                .HasDatabaseName("IX_POST_EXCLUIDO");

            builder.HasOne(p => p.Autor)
                .WithMany()
                .HasForeignKey(p => p.AutorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(p => !p.Excluido);
        }
    }
}
