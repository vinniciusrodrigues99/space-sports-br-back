using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.JogadorFoto
{
    public class JogadorFotoConfiguration : IEntityTypeConfiguration<Domain.Entities.JogadorFoto.JogadorFoto>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.JogadorFoto.JogadorFoto> builder)
        {
            builder.ToTable("T_JOGADOR_FOTO");

            builder.HasKey(j => j.Id);
            builder.Property(j => j.Id).HasColumnName("ID_JOGADOR_FOTO");

            builder.Property(j => j.PlayerId)
                .HasColumnName("TX_PLAYER_ID")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(j => j.PlayerName)
                .HasColumnName("TX_PLAYER_NAME")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(j => j.NomeArquivo)
                .HasColumnName("TX_NOME_ARQUIVO")
                .IsRequired()
                .HasMaxLength(260);

            builder.Property(j => j.AtualizadoPorUserId)
                .HasColumnName("ID_USUARIO");

            builder.Property(j => j.CriadoEm)
                .HasColumnName("DT_CRIADO_EM")
                .IsRequired();

            builder.Property(j => j.CriadoPor)
                .HasColumnName("TX_CRIADO_POR")
                .HasMaxLength(100);

            builder.Property(j => j.DataModificacao)
                .HasColumnName("DT_MODIFICACAO");

            builder.Property(j => j.ModificadoPor)
                .HasColumnName("TX_MODIFICADO_POR")
                .HasMaxLength(100);

            builder.Property(j => j.Excluido)
                .HasColumnName("FL_EXCLUIDO")
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(j => j.PlayerId)
                .IsUnique()
                .HasDatabaseName("IX_JOGADOR_FOTO_PLAYER_ID");

            builder.HasQueryFilter(j => !j.Excluido);
        }
    }
}
