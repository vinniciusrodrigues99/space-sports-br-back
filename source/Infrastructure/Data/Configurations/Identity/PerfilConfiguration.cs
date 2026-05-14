using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Infrastructure.Data.Configurations.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Identity
{
    public class PerfilConfiguration : IEntityTypeConfiguration<Perfil>
    {
        public void Configure(EntityTypeBuilder<Perfil> builder)
        {

            builder.ToTable("T_PERFIL");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .HasColumnName("ID_PERFIL");

            builder.Property(r => r.Name)
                .HasColumnName("NM_PERFIL")
                .HasMaxLength(256);

            builder.Property(r => r.NormalizedName)
                .HasColumnName("NM_PERFIL_NORMALIZADO")
                .HasMaxLength(256);

            builder.Property(r => r.ConcurrencyStamp)
                .HasColumnName("TX_CONCURRENCY_STAMP");
            
            builder.Property(r => r.CriadoPor)
                .HasColumnName("TX_CRIADO_POR")
                .HasMaxLength(100);

            builder.Property(r => r.CriadoEm)
                .HasColumnName("DT_CRIADO_EM")
                .IsRequired();

            builder.Property(r => r.ModificadoPor)
                .HasColumnName("TX_MODIFICADO_POR")
                .HasMaxLength(100);

            builder.Property(r => r.DataModificacao)
                .HasColumnName("DT_MODIFICACAO");

            builder.Property(r => r.Excluido)
                .HasColumnName("FL_EXCLUIDO")
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(r => r.NormalizedName)
                .IsUnique()
                .HasDatabaseName("IX_PERFIL_NOME_NORMALIZADO");

            builder.HasIndex(r => r.Excluido)
                .HasDatabaseName("IX_PERFIL_EXCLUIDO");

            // Query filter para soft delete
            builder.HasQueryFilter(r => !r.Excluido);
        }
    }
}
