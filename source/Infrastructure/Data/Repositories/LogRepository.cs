using System.Data;
using Dapper;
using FSP.Api.Application.Features.Logs.DTOs;
using FSP.Api.Domain.Entities.Log;
using FSP.Api.Domain.Interfaces.Data.Repositories;
using FSP.Api.Infrastructure.Data.DbContexts;
using FSP.Api.Infrastructure.Data.Respositories;
using Microsoft.Extensions.Configuration;

namespace FSP.Api.Infrastructure.Data.Repositories
{
    public class LogRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        : RepositoryBase<Logs>(dbContext, configuration), ILogRepository
    {
        public async Task<(List<object> Logs, int TotalRegistros)> BuscarLogsPorFiltrosAsync(
            DateTime dataInicio,
            DateTime dataFim,
            string? buscaUsuario,
            int pagina,
            int limiteLinhas,
            CancellationToken cancellationToken = default)
        {
            using var connection = CreateConnection();

            var query = @"
                SELECT
                    l.""ID_LOG"" AS Id,
                    l.""TX_ENDERECO_IP"" AS EnderecoIp,
                    l.""TX_AGENTE_USUARIO"" AS AgenteUsuario,
                    l.""TX_URL"" AS Url,
                    l.""TX_DESCRICAO"" AS Descricao,
                    l.""TX_EMAIL_USUARIO"" AS EmailUsuario,
                    l.""NM_USUARIO"" AS NomeUsuario,
                    l.""DT_DATA_HORA"" AS DataHora,
                    l.""TX_DETALHES"" AS Detalhes
                FROM ""T_LOG"" l
                WHERE l.""DT_DATA_HORA"" >= @DataInicio
                  AND l.""DT_DATA_HORA"" <= @DataFim";

            var parameters = new DynamicParameters();
            parameters.Add("DataInicio", dataInicio);
            parameters.Add("DataFim", dataFim);

            if (!string.IsNullOrWhiteSpace(buscaUsuario) && buscaUsuario.Length >= 2)
            {
                query += @"
                  AND (l.""NM_USUARIO"" ILIKE @BuscaUsuario OR l.""TX_EMAIL_USUARIO"" ILIKE @BuscaUsuario)";
                parameters.Add("BuscaUsuario", $"%{buscaUsuario}%");
            }

            var countQuery = $@"SELECT COUNT(1) FROM ({query}) AS count_query";
            var totalRegistros = await connection.QuerySingleAsync<int>(countQuery, parameters);

            var finalQuery = $@"
                {query}
                ORDER BY l.""DT_DATA_HORA"" DESC
                LIMIT @PageSize OFFSET @Offset";

            parameters.Add("Offset", (pagina - 1) * limiteLinhas);
            parameters.Add("PageSize", limiteLinhas);

            var logs = await connection.QueryAsync<LogDto>(finalQuery, parameters);
            return (logs.Cast<object>().ToList(), totalRegistros);
        }

        public async Task<object?> BuscarLogPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var connection = CreateConnection();

            var query = @"
                SELECT
                    l.""ID_LOG"" AS Id,
                    l.""TX_ENDERECO_IP"" AS EnderecoIp,
                    l.""TX_AGENTE_USUARIO"" AS AgenteUsuario,
                    l.""TX_URL"" AS Url,
                    l.""TX_DESCRICAO"" AS Descricao,
                    l.""TX_EMAIL_USUARIO"" AS EmailUsuario,
                    l.""NM_USUARIO"" AS NomeUsuario,
                    l.""DT_DATA_HORA"" AS DataHora,
                    l.""TX_DETALHES"" AS Detalhes
                FROM ""T_LOG"" l
                WHERE l.""ID_LOG"" = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            return await connection.QueryFirstOrDefaultAsync<LogDto>(query, parameters);
        }
    }
}
