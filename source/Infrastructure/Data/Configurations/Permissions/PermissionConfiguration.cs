using FSP.Api.Domain.Entities.BaseAuditable;
using FSP.Api.Domain.Entities.Permission;
using FSP.Api.Infrastructure.Data.Configurations.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Permissions
{
    public class PermissionConfiguration : AuditableEntityConfiguration<Permissao>
    {
        public override void Configure(EntityTypeBuilder<Permissao> builder)
        {
            builder.ToTable("T_PERMISSAO");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("ID_PERMISSAO");

            builder.Property(p => p.Nome)
                .HasColumnName("NM_PERMISSAO")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.NomeExibicao)
                .HasColumnName("NM_EXIBICAO")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Descricao)
                .HasColumnName("DS_PERMISSAO")
                .HasMaxLength(500);

            builder.Property(p => p.Categoria)
                .HasColumnName("CTG_PERMISSAO")
                .HasMaxLength(50);

            // Configure indexes
            builder.HasIndex(p => p.Nome).IsUnique();
            builder.HasIndex(p => p.Categoria);
            builder.HasIndex(p => p.Excluido);

        }
    }
}
