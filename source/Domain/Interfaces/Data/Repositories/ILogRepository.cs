namespace FSP.Api.Domain.Interfaces.Data.Repositories
{
    public interface ILogRepository
    {
        Task<(List<object> Logs, int TotalRegistros)> BuscarLogsPorFiltrosAsync(
            DateTime dataInicio, 
            DateTime dataFim, 
            string? buscaUsuario, 
            int pagina, 
            int limiteLinhas,
            CancellationToken cancellationToken = default);
        
        Task<object?> BuscarLogPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
