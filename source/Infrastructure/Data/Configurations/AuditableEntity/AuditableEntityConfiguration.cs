using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.AuditableEntity
{
    public abstract class AuditableEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property<DateTimeOffset>("CriadoEm")
                .HasColumnName("DT_CRIADO_EM")
                .IsRequired();

            builder.Property<string>("CriadoPor")
                .HasColumnName("TX_CRIADO_POR")
                .HasMaxLength(256);

            builder.Property<DateTimeOffset>("DataModificacao")
                .HasColumnName("DT_MODIFICACAO")
                .IsRequired();

            builder.Property<string>("ModificadoPor")
                .HasColumnName("TX_MODIFICADO_POR")
                .HasMaxLength(256);

            builder.Property<bool>("Excluido")
                .HasColumnName("FL_EXCLUIDO")
                .IsRequired()
                .HasDefaultValue(false);

            // Query filter para soft delete
            builder.HasQueryFilter(e => !EF.Property<bool>(e, "Excluido"));
        }
    }
}