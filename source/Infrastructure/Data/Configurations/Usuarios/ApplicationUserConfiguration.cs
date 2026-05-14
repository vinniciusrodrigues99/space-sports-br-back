using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Infrastructure.Data.Configurations.AuditableEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Usuarios
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser> 
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            // Tabela
            builder.ToTable("T_USUARIO");
            
            // Chave primária
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName("ID_USUARIO");

            // Campos do Identity
            builder.Property(u => u.UserName)
                .HasColumnName("NM_USUARIO")
                .HasMaxLength(256);

            builder.Property(u => u.NormalizedUserName)
                .HasColumnName("NM_USUARIO_NORMALIZADO")
                .HasMaxLength(256);

            builder.Property(u => u.Email)
                .HasColumnName("TX_EMAIL")
                .HasMaxLength(256);

            builder.Property(u => u.NormalizedEmail)
                .HasColumnName("TX_EMAIL_NORMALIZADO")
                .HasMaxLength(256);

            builder.Property(u => u.EmailConfirmed)
                .HasColumnName("FL_EMAIL_CONFIRMADO");

            builder.Property(u => u.PasswordHash)
                .HasColumnName("TX_SENHA_HASH");

            builder.Property(u => u.SecurityStamp)
                .HasColumnName("TX_SECURITY_STAMP");

            builder.Property(u => u.ConcurrencyStamp)
                .HasColumnName("TX_CONCURRENCY_STAMP");

            builder.Property(u => u.PhoneNumber)
                .HasColumnName("NR_TELEFONE_IDENTITY")
                .HasMaxLength(20);

            builder.Property(u => u.PhoneNumberConfirmed)
                .HasColumnName("FL_TELEFONE_CONFIRMADO");

            builder.Property(u => u.TwoFactorEnabled)
                .HasColumnName("FL_TWO_FACTOR_HABILITADO");

            builder.Property(u => u.LockoutEnd)
                .HasColumnName("DT_FIM_BLOQUEIO");

            builder.Property(u => u.LockoutEnabled)
                .HasColumnName("FL_BLOQUEIO_HABILITADO");

            builder.Property(u => u.AccessFailedCount)
                .HasColumnName("QTD_TENTATIVAS_FALHAS");

            // Campos customizados
            builder.Property(u => u.NomeCompleto)
                .HasColumnName("NM_COMPLETO")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.CPF)
                .HasColumnName("TX_CPF")
                .HasMaxLength(14);

            builder.Property(u => u.TxFoto)
                .HasColumnName("TX_FOTO")
                .HasMaxLength(500);

            builder.Property(u => u.Telefone)
                .HasColumnName("NR_TELEFONE")
                .HasMaxLength(20);

            builder.Property(u => u.NotificacaoEmail)
                .HasColumnName("FL_NOTIFICACAO_EMAIL")
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.NotificacaoSMS)
                .HasColumnName("FL_NOTIFICACAO_SMS")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.CanPublish)
                .HasColumnName("FL_CAN_PUBLISH")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.RefreshToken)
                .HasColumnName("TX_REFRESH_TOKEN")
                .HasMaxLength(500);
            
            builder.Property(u => u.DataExpiracaoRefreshToken)
                .HasColumnName("DT_EXPIRACAO_REFRESH_TOKEN");
            
            builder.Property(u => u.PrimeiroTokenAcesso)
                .HasColumnName("TX_PRIMEIRO_TOKEN_ACESSO")
                .HasMaxLength(500);

            builder.Property(u => u.ExpiracaoPrimeiroAcesso)
                .HasColumnName("DT_EXPIRACAO_PRIMEIRO_ACESSO");

            builder.Property(u => u.CriadoPor)
                .HasColumnName("TX_CRIADO_POR")
                .HasMaxLength(100);

            builder.Property(u => u.CriadoEm)
                .HasColumnName("DT_CRIADO_EM")
                .IsRequired();

            builder.Property(u => u.ModificadoPor)
                .HasColumnName("TX_MODIFICADO_POR")
                .HasMaxLength(100);

            builder.Property(u => u.DataModificacao)
                .HasColumnName("DT_MODIFICACAO");

            builder.Property(u => u.Excluido)
                .HasColumnName("FL_EXCLUIDO")
                .IsRequired()
                .HasDefaultValue(false);
                
            // Índices

            builder.HasIndex(u => u.Email)
                .HasDatabaseName("IX_USUARIO_EMAIL");

            builder.HasIndex(u => u.NormalizedEmail)
                .HasDatabaseName("IX_USUARIO_EMAIL_NORMALIZADO");

            builder.HasIndex(u => u.NormalizedUserName)
                .HasDatabaseName("IX_USUARIO_NOME_NORMALIZADO");

            builder.HasIndex(u => u.Excluido)
                .HasDatabaseName("IX_USUARIO_EXCLUIDO");

            // Query filter para soft delete
            builder.HasQueryFilter(u => !u.Excluido);
        }
    }
}