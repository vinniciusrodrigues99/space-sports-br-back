using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Palpite
{
    public class PalpiteConfiguration : IEntityTypeConfiguration<Domain.Entities.Palpite.Palpite>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Palpite.Palpite> builder)
        {
            builder.ToTable("T_PALPITE");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("ID_PALPITE");

            builder.Property(p => p.EventId)
                .HasColumnName("ID_EVENTO")
                .IsRequired();

            builder.Property(p => p.HomeName)
                .HasColumnName("TX_TIME_CASA")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.AwayName)
                .HasColumnName("TX_TIME_FORA")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.HomeGoals)
                .HasColumnName("QTD_GOLS_CASA")
                .IsRequired();

            builder.Property(p => p.AwayGoals)
                .HasColumnName("QTD_GOLS_FORA")
                .IsRequired();

            builder.Property(p => p.Nickname)
                .HasColumnName("TX_APELIDO")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.CriadoEm)
                .HasColumnName("DT_CRIADO_EM")
                .IsRequired();

            builder.Property(p => p.CriadoPor)
                .HasColumnName("TX_CRIADO_POR")
                .HasMaxLength(100);

            builder.Property(p => p.DataModificacao)
                .HasColumnName("DT_MODIFICACAO");

            builder.Property(p => p.ModificadoPor)
                .HasColumnName("TX_MODIFICADO_POR")
                .HasMaxLength(100);

            builder.Property(p => p.Excluido)
                .HasColumnName("FL_EXCLUIDO")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.UserId)
                .HasColumnName("ID_USUARIO");

            builder.Property(p => p.Stage)
                .HasColumnName("TX_FASE")
                .HasMaxLength(60);

            // Um palpite por nickname por jogo
            builder.HasIndex(p => new { p.EventId, p.Nickname })
                .IsUnique()
                .HasDatabaseName("IX_PALPITE_EVENTO_APELIDO");

            builder.HasQueryFilter(p => !p.Excluido);
        }
    }
}
