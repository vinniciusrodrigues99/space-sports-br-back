namespace FSP.Api.Application.Interfaces
{
    public interface ILogService
    {
        /// <summary>
        /// Registra log de informação
        /// </summary>
        Task Information(
            string descricao,
            string detalhes);

        /// <summary>
        /// Registra log de aviso
        /// </summary>
        Task Warning(
            string descricao,
            string detalhes);

        /// <summary>
        /// Registra log de erro
        /// </summary>
        Task Error(
            string descricao,
            string detalhes,
            Exception? exception = null);
    }
}