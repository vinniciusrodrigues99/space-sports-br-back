using FSP.Api.Domain.Entities.Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Api.Infrastructure.Data.Configurations.Log
{
    public class LogConfiguration : IEntityTypeConfiguration<Logs>
    {
        public void Configure(EntityTypeBuilder<Logs> builder)
        {
            builder.ToTable("T_LOG");

            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id)
                .HasColumnName("ID_LOG")
                .ValueGeneratedOnAdd();

            builder.Property(l => l.EnderecoIp)
                .HasColumnName("TX_ENDERECO_IP")
                .HasMaxLength(50);

            builder.Property(l => l.AgenteUsuario)
                .HasColumnName("TX_AGENTE_USUARIO")
                .HasMaxLength(500);

            builder.Property(l => l.Url)
                .HasColumnName("TX_URL")
                .HasMaxLength(500);

            builder.Property(l => l.Descricao)
                .HasColumnName("TX_DESCRICAO")
                .HasMaxLength(1000);

            builder.Property(l => l.EmailUsuario)
                .HasColumnName("TX_EMAIL_USUARIO")
                .HasMaxLength(256);

            builder.Property(l => l.NomeUsuario)
                .HasColumnName("NM_USUARIO")
                .HasMaxLength(256);

            builder.Property(l => l.DataHora)
                .HasColumnName("DT_DATA_HORA");

            builder.Property(l => l.Detalhes)
                .HasColumnName("TX_DETALHES");

            builder.HasIndex(l => l.DataHora)
                .HasDatabaseName("IX_LOG_DATA_HORA");

            builder.HasIndex(l => l.EmailUsuario)
                .HasDatabaseName("IX_LOG_EMAIL_USUARIO");

            builder.HasIndex(l => l.NomeUsuario)
                .HasDatabaseName("IX_LOG_NOME_USUARIO");
        }
    }
}
