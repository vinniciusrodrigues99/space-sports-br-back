using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSP.Api.Application.Features.Permission.Queries.GetPermissions;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/permissao")] 
    public class PermissionController(
        INotificationHandler<DomainNotification> notifications,
        INotificationHandler<DomainSuccesNotification> succesNotifications,
        IMediator mediatorHandler) : BaseController(notifications, succesNotifications, mediatorHandler)
    {
        private readonly IMediator _mediatorHandler = mediatorHandler;

        /// <summary>
        /// Obtém todas as permissões disponíveis no sistema
        /// </summary>
        /// <param name="categoria">Filtrar por categoria (opcional)</param>
        /// <param name="ativo">Filtrar por status ativo (opcional)</param>
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<GetPermissionsQueryResponse>))]
        [HttpGet]
        public async Task<IActionResult> ObterPermissoes(
            [FromQuery] string? categoria = null, 
            [FromQuery] bool? ativo = null)
        {
            var query = new GetPermissionsQuery(categoria, ativo);
            var result = await _mediatorHandler.Send(query);
            return Response(result);
        }

        /// <summary>
        /// Obtém todas as categorias de permissões disponíveis
        /// </summary>
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<GetPermissionsQueryResponse>))]
        [HttpGet("categorias")]
        public async Task<IActionResult> ObterCategoriasPermissoes()
            => Response(await _mediatorHandler.Send(new GetPermissionsQuery()));

        /// <summary>
        /// Popula o banco de dados com as permissões padrão do sistema
        /// </summary>
        // [Authorize(Roles = $"{ProfileRoles.Admin}")]
    }
}
