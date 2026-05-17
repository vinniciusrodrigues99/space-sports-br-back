using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSP.Api.Application.Features.RoleManagement.Commands.CreateRole;
using FSP.Api.Application.Features.RoleManagement.Commands.AssignRoleToUser;
using FSP.Api.Application.Features.RoleManagement.Commands.AssignPermissionToRole;
using FSP.Api.Application.Features.RoleManagement.Queries.GetRoles;
using FSP.Api.Application.Features.RoleManagement.Queries.GetUserRoles;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;
using FSP.Api.Domain.Constants;

namespace FSP.Api.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/gerenciamento-roles")]
    public class RoleManagementController(
        INotificationHandler<DomainNotification> notifications,
        INotificationHandler<DomainSuccesNotification> succesNotifications,
        IMediator mediatorHandler) : BaseController(notifications, succesNotifications, mediatorHandler)
    {
        private readonly IMediator _mediatorHandler = mediatorHandler;

        /// <summary>
        /// Obtém todas as roles disponíveis
        /// </summary>
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<GetRolesQueryResponse>))]
        [HttpGet("roles")]
        public async Task<IActionResult> ObterRoles()
            => Response(await _mediatorHandler.Send(new GetRolesQuery()));

        /// <summary>
        /// Obtém as roles de um usuário específico
        /// </summary>
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<GetUserRolesQueryResponse>))]
        [HttpGet("usuario/{idUsuario}/roles")]
        public async Task<IActionResult> ObterRolesUsuario(Guid idUsuario)
            => Response(await _mediatorHandler.Send(new GetUserRolesQuery(idUsuario)));

        /// <summary>
        /// Cria uma nova role
        /// </summary>
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = $"{ProfileRoles.Admin}")]
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<CreateRoleCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<object>))]
        [HttpPost("roles")]
        public async Task<IActionResult> CriarRole([FromBody] CreateRoleRequest request)
            => Response(await _mediatorHandler.Send(new CreateRoleCommand(request)));

        /// <summary>
        /// Atribui uma role a um usuário
        /// </summary>
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = ProfileRoles.Admin)]
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<AssignRoleToUserCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<object>))]
        [HttpPost("usuario/{idUsuario}/atribuir-role")]
        public async Task<IActionResult> AtribuirRoleUsuario(Guid idUsuario, [FromBody] AssignRoleToUserRequest request)
            => Response(await _mediatorHandler.Send(new AssignRoleToUserCommand(idUsuario, request)));

        /// <summary>
        /// Atribui uma permissão a uma role
        /// </summary>
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = $"{ProfileRoles.Admin}")]
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<AssignPermissionToRoleCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<object>))]
        [HttpPost("role/{nomeRole}/atribuir-permissao")]
        public async Task<IActionResult> AtribuirPermissaoRole(string nomeRole, [FromBody] AssignPermissionToRoleRequest request)
            => Response(await _mediatorHandler.Send(new AssignPermissionToRoleCommand(nomeRole, request)));
    }
}
