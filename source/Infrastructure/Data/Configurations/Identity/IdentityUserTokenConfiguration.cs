using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Identity
{
    public class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
        {
            builder.ToTable("T_USUARIO_TOKEN");

            builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            builder.Property(ut => ut.UserId)
                .HasColumnName("ID_USUARIO");

            builder.Property(ut => ut.LoginProvider)
                .HasColumnName("TX_PROVEDOR_LOGIN")
                .HasMaxLength(128);

            builder.Property(ut => ut.Name)
                .HasColumnName("NM_TOKEN")
                .HasMaxLength(128);

            builder.Property(ut => ut.Value)
                .HasColumnName("TX_VALOR_TOKEN");
        }
    }
}
