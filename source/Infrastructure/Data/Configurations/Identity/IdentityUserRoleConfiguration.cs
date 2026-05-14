using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Identity
{
    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
        {
            builder.ToTable("T_USUARIO_PERFIL");

            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.Property(ur => ur.UserId)
                .HasColumnName("ID_USUARIO");

            builder.Property(ur => ur.RoleId)
                .HasColumnName("ID_PERFIL");

            builder.HasIndex(ur => ur.UserId)
                .HasDatabaseName("IX_USUARIO_PERFIL_ID_USUARIO");

            builder.HasIndex(ur => ur.RoleId)
                .HasDatabaseName("IX_USUARIO_PERFIL_ID_PERFIL");
        }
    }
}
