using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;
using FSP.Api.Domain.Entities.Log;
using Microsoft.AspNetCore.Http;

namespace FSP.Api.Infrastructure.Services
{
    public class LogService(
        IApplicationDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService) : ILogService
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task Information(string descricao, string detalhes)
        {
            await CriarLog(descricao, detalhes);
        }

        public async Task Warning(string descricao, string detalhes)
        {
            await CriarLog(descricao, $"[WARNING] {detalhes}");
        }

        public async Task Error(string descricao, string detalhes, Exception? exception = null)
        {
            var detalhesCompletos = exception != null
                ? $"[ERROR] {detalhes}\n\nException: {exception.Message}\n\nStackTrace: {exception.StackTrace}"
                : $"[ERROR] {detalhes}";

            await CriarLog(descricao, detalhesCompletos);
        }

        private async Task CriarLog(string descricao, string detalhes)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var log = new Logs()
            {
                EnderecoIp = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "N/A",
                AgenteUsuario = httpContext?.Request?.Headers["User-Agent"].ToString() ?? "N/A",
                Url = httpContext?.Request?.Path.ToString() ?? "N/A",
                Descricao = descricao,
                EmailUsuario = _currentUserService.UserEmail ?? "Sistema",
                NomeUsuario = _currentUserService.UserName ?? "Sistema",
                DataHora = DateTimeOffset.UtcNow,
                Detalhes = detalhes
            };

            _dbContext.Logs.Add(log);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
    }
}
