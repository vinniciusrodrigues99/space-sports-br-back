using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSP.Api.Application.Features.Autenticacao.Commands.Login;
using FSP.Api.Application.Features.Autenticacao.Commands.RefreshToken;
using FSP.Api.Application.Features.Autenticacao.Commands.SendTwoFactorCode;
using FSP.Api.Application.Features.Autenticacao.Commands.VerifyTwoFactorCode;
using FSP.Api.Application.Features.Autenticacao.Commands.RequestResetTokenPassword;
using FSP.Api.Application.Features.Autenticacao.Commands.ResetPassword;
using FSP.Api.Application.Features.Usuarios.Commands.SetupFirstAccess;
using FSP.Api.Application.Features.Usuarios.Commands.ResendFirstAccess;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.WebApi.Controllers
{
    [ApiController]
    [Route("api/autenticacao")]
    public class AuthController(INotificationHandler<DomainNotification> notifications,
                          INotificationHandler<DomainSuccesNotification> succesNotifications,
                          IMediator mediatorHandler) : BaseController(notifications, succesNotifications, mediatorHandler)
    {
        private readonly IMediator _mediatorHandler = mediatorHandler;

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<LoginCommandResponse>))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        => Response(await _mediatorHandler.Send(new LoginCommand(request)));

        [HttpPost("renovar-token")]
        public async Task<IActionResult> RenovarToken([FromBody] RefreshTokenRequest request)
        => Response(await _mediatorHandler.Send(new RefreshTokenCommand(request)));

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<VerifyTwoFactorCodeCommandResponse>))]
        [HttpPost("verificar-codigo-2fa")]
        public async Task<IActionResult> VerificarCodigo2FA([FromBody] VerifyTwoFactorCodeRequest request)
        => Response(await _mediatorHandler.Send(new VerifyTwoFactorCodeCommand(request)));

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<SendTwoFactorCodeCommandResponse>))]
        [HttpPost("reenviar-codigo-2fa")]
        public async Task<IActionResult> ReenviarCodigo2FA([FromBody] SendTwoFactorCodeRequest request)
        => Response(await _mediatorHandler.Send(new SendTwoFactorCodeCommand(request)));

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<RequestResetTokenPasswordCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPost("solicitar-reset-senha")]
        public async Task<IActionResult> SolicitarResetSenha([FromBody] RequestResetTokenPasswordRequest request)
        {
            var token = await _mediatorHandler.Send(new RequestResetTokenPasswordCommand(request));
            return Response(token);
        }

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<ResetPasswordCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPost("resetar-senha")]
        public async Task<IActionResult> ResetarSenha([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediatorHandler.Send(new ResetPasswordCommand(request));
            return Response(result);
        }

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<SetupFirstAccessCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPost("primeiro-acesso")]
        public async Task<IActionResult> PrimeiroAcesso([FromBody] SetupFirstAccessRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediatorHandler.Send(new SetupFirstAccessCommand(request));
            return Response(result);
        }

        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<ResendFirstAcessCommandResponse>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPost("reenviar-primeiro-acesso/{idUsuario}")]
        public async Task<IActionResult> ReenviarPrimeiroAcesso([FromRoute] Guid idUsuario)
        {
            var result = await _mediatorHandler.Send(new ResendFirstAccessCommand(idUsuario));
            return Response(result);
        }
    }
}
