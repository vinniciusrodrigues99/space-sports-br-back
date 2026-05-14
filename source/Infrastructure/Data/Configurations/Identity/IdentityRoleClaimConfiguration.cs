using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Identity
{
    public class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
        {
            builder.ToTable("T_PERFIL_CLAIM");

            builder.HasKey(rc => rc.Id);
            builder.Property(rc => rc.Id)
                .HasColumnName("ID_PERFIL_CLAIM");

            builder.Property(rc => rc.RoleId)
                .HasColumnName("ID_PERFIL");

            builder.Property(rc => rc.ClaimType)
                .HasColumnName("TX_TIPO_CLAIM")
                .HasMaxLength(256);

            builder.Property(rc => rc.ClaimValue)
                .HasColumnName("TX_VALOR_CLAIM");

            builder.HasIndex(rc => rc.RoleId)
                .HasDatabaseName("IX_PERFIL_CLAIM_ID_PERFIL");
        }
    }
}
