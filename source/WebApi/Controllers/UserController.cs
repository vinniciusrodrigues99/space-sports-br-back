using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSP.Api.Application.Features.Usuarios.Commands.CreateUser;
using FSP.Api.Application.Features.Usuarios.Commands.DeleteUser;
using FSP.Api.Application.Features.Usuarios.Queries.GetUser;
using FSP.Api.Application.Features.Usuarios.Commands.UpdateUser;
using FSP.Api.Application.Features.Usuarios.Commands.UpdateUserProfile;
using FSP.Api.Application.Features.Usuarios.Commands.ChangePassword;
using FSP.Api.Application.Features.Usuarios.Commands.RequestChangePasswordCode;
using FSP.Api.Application.Features.Usuarios.Commands.UploadProfilePhoto;
using FSP.Api.Application.Features.Usuarios.Queries.GetUserProfile;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;
using System.Security.Claims;

namespace FSP.Api.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/usuario")]
    public class UserController(INotificationHandler<DomainNotification> notifications,
                          INotificationHandler<DomainSuccesNotification> succesNotifications,
                          IMediator mediator) : BaseController(notifications, succesNotifications, mediator)
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<CreateUserCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [ProducesResponseType(statusCode: 403, type: typeof(ResponseBase<>))]
        [HttpPost]
        public async Task<IActionResult> CriarUsuario(CreateUserRequest request)
        => Response(await _mediator.Send(new CreateUserCommand(request)));

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<GetUserQueryResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> BuscarUsuario(Guid idUsuario)
        {
            var query = new GetUserQuery { UserID = idUsuario };
            var user = await _mediator.Send(query);
            return Response(user);
        }

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<UpdateUserCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPut]
        public async Task<IActionResult> AtualizarUsuario([FromQuery] Guid idUsuario, UpdateUserRequest updateUserRequest)
        {
            var result = await _mediator.Send(new UpdateUserCommand(idUsuario, updateUserRequest));
            if (result is null)
            {
                return NotFound();
            }
            return Response(result);
        }

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<UpdateUserCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpDelete("{idUsuario}")]
        public async Task<IActionResult> ExcluirUsuario(Guid idUsuario)
        {
            var command = new DeleteUserCommand { UserID = idUsuario };
            return Response(await _mediator.Send(command));
        }

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<GetUserProfileQueryResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpGet("perfil")]
        public async Task<IActionResult> ObterPerfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetUserProfileQuery(Guid.Parse(userId)));
            return Response(result);
        }

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<UpdateUserProfileCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPut("perfil")]
        public async Task<IActionResult> AtualizarPerfil([FromBody] UpdateUserProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new UpdateUserProfileCommand(Guid.Parse(userId), request));
            return Response(result);
        }   

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<RequestChangePasswordCodeCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPost("perfil/senha/solicitar-codigo")]
        public async Task<IActionResult> SolicitarCodigoAlteracaoSenha([FromBody] RequestChangePasswordCodeRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new RequestChangePasswordCodeCommand(Guid.Parse(userId), request));
            return Response(result);
        }

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<ChangePasswordCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPut("perfil/senha")]
        public async Task<IActionResult> AlterarSenha([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new ChangePasswordCommand(Guid.Parse(userId), request));
            return Response(result);
        }

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<UploadProfilePhotoCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPost("perfil/foto")]
        public async Task<IActionResult> UploadFotoPerfil([FromForm] IFormFile foto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (foto == null || foto.Length == 0)
            {
                return BadRequest(new { message = "Nenhuma foto foi enviada." });
            }

            var result = await _mediator.Send(new UploadProfilePhotoCommand(Guid.Parse(userId), foto));
            return Response(result);
        }
    }
}
