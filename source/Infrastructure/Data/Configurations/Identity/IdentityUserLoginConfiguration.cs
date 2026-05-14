using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Identity
{
    public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
        {
            builder.ToTable("T_USUARIO_LOGIN");

            builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            builder.Property(ul => ul.LoginProvider)
                .HasColumnName("TX_PROVEDOR_LOGIN")
                .HasMaxLength(128);

            builder.Property(ul => ul.ProviderKey)
                .HasColumnName("TX_CHAVE_PROVEDOR")
                .HasMaxLength(128);

            builder.Property(ul => ul.ProviderDisplayName)
                .HasColumnName("NM_EXIBICAO_PROVEDOR")
                .HasMaxLength(256);

            builder.Property(ul => ul.UserId)
                .HasColumnName("ID_USUARIO");

            builder.HasIndex(ul => ul.UserId)
                .HasDatabaseName("IX_USUARIO_LOGIN_ID_USUARIO");
        }
    }
}
