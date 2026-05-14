/*
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSP.Api.Application.Features.Logs.Queries.GetLogs;
using FSP.Api.Application.Features.Logs.Queries.GetLogById;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/logs")]
    public class LogsController(INotificationHandler<DomainNotification> notifications,
                          INotificationHandler<DomainSuccesNotification> succesNotifications,
                          IMediator mediator) : BaseController(notifications, succesNotifications, mediator)
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [Authorize(Policy = "Logs.Leitura")]
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<GetLogsQueryResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [ProducesResponseType(statusCode: 403, type: typeof(ResponseBase<>))]
        [HttpGet]
        public async Task<IActionResult> BuscarLogs(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] string? buscaUsuario = null,
            [FromQuery] int pagina = 1,
            [FromQuery] int limiteLinhas = 20)
        {
            var query = new GetLogsQuery
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                BuscaUsuario = buscaUsuario,
                Pagina = pagina,
                LimiteLinhas = limiteLinhas
            };

            return Response(await _mediator.Send(query));
        }

        [Authorize(Policy = "Logs.Leitura")]
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<GetLogByIdQueryResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [ProducesResponseType(statusCode: 403, type: typeof(ResponseBase<>))]
        [ProducesResponseType(statusCode: 404, type: typeof(ResponseBase<>))]
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarLogPorId(Guid id)
        {
            var query = new GetLogByIdQuery(id);
            return Response(await _mediator.Send(query));
        }
    }
}
*/
