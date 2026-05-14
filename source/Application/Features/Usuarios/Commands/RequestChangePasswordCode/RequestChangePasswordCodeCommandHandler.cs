using Microsoft.AspNetCore.Identity;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Usuarios.Commands.RequestChangePasswordCode;

public class RequestChangePasswordCodeCommandHandler(
    UserManager<ApplicationUser> _userManager,
    IEmailService _emailService,
    ISmsService _smsService,
    IMediator _mediator) : IRequestHandler<RequestChangePasswordCodeCommand, ResponseBase<RequestChangePasswordCodeCommandResponse>>
{
    public async Task<ResponseBase<RequestChangePasswordCodeCommandResponse>> Handle(RequestChangePasswordCodeCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var user = await _userManager.FindByIdAsync(command.IdUsuario.ToString());
        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("RequestChangePasswordCode", "Usuário não localizado."), cancellationToken);
            return ResponseBase<RequestChangePasswordCodeCommandResponse>.Failure();
        }

        var provider = request.MetodoEnvio.Equals("email", StringComparison.CurrentCultureIgnoreCase) ? "Email" : "Phone";
        var code = await _userManager.GenerateTwoFactorTokenAsync(user, provider);

        try
        {
            if (request.MetodoEnvio.Equals("email", StringComparison.CurrentCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(user.Email))
                {
                    await _mediator.Publish(new DomainNotification("RequestChangePasswordCode", "Usuário não possui email cadastrado."), cancellationToken);
                    return ResponseBase<RequestChangePasswordCodeCommandResponse>.Failure();
                }
                await _emailService.SendTwoFactorCodeAsync(user.Email, code);
            }
            else
            {
                if (string.IsNullOrEmpty(user.PhoneNumber))
                {
                    await _mediator.Publish(new DomainNotification("RequestChangePasswordCode", "Usuário não possui telefone cadastrado."), cancellationToken);
                    return ResponseBase<RequestChangePasswordCodeCommandResponse>.Failure();
                }
                await _smsService.SendTwoFactorCodeAsync(user.PhoneNumber, code);
            }

            var expiresAt = DateTime.Now.AddMinutes(5);

            return ResponseBase<RequestChangePasswordCodeCommandResponse>.Success(new RequestChangePasswordCodeCommandResponse
            {
                Message = $"Código de verificação enviado para {(request.MetodoEnvio.ToLower() == "email" ? "seu email" : "seu telefone")}.",
                ExpiresAt = expiresAt
            });
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new DomainNotification("RequestChangePasswordCode", $"Erro ao enviar código: {ex.Message}"), cancellationToken);
            return ResponseBase<RequestChangePasswordCodeCommandResponse>.Failure();
        }
    }
}
