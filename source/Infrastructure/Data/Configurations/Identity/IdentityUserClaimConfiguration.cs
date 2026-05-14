using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Identity
{
    public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
        {
            builder.ToTable("T_USUARIO_CLAIM");

            builder.HasKey(uc => uc.Id);
            builder.Property(uc => uc.Id)
                .HasColumnName("ID_USUARIO_CLAIM");

            builder.Property(uc => uc.UserId)
                .HasColumnName("ID_USUARIO");

            builder.Property(uc => uc.ClaimType)
                .HasColumnName("TX_TIPO_CLAIM")
                .HasMaxLength(256);

            builder.Property(uc => uc.ClaimValue)
                .HasColumnName("TX_VALOR_CLAIM");

            builder.HasIndex(uc => uc.UserId)
                .HasDatabaseName("IX_USUARIO_CLAIM_ID_USUARIO");
        }
    }
}
