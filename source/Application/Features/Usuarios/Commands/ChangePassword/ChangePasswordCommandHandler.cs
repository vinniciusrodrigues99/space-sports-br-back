using Microsoft.AspNetCore.Identity;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Usuarios.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    UserManager<ApplicationUser> _userManager,
    IMediator _mediator) : IRequestHandler<ChangePasswordCommand, ResponseBase<ChangePasswordCommandResponse>>
{
    public async Task<ResponseBase<ChangePasswordCommandResponse>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (request.NovaSenha != request.ConfirmarNovaSenha)
        {
            await _mediator.Publish(new DomainNotification("ChangePassword", "As senhas não coincidem."), cancellationToken);
            return ResponseBase<ChangePasswordCommandResponse>.Failure();
        }

        var user = await _userManager.FindByIdAsync(command.IdUsuario.ToString());
        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("ChangePassword", "Usuário não localizado."), cancellationToken);
            return ResponseBase<ChangePasswordCommandResponse>.Failure();
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.SenhaAtual);
        if (!isPasswordValid)
        {
            await _mediator.Publish(new DomainNotification("ChangePassword", "Senha atual incorreta."), cancellationToken);
            return ResponseBase<ChangePasswordCommandResponse>.Failure();
        }

        var provider = request.MetodoEnvio.Equals("email", StringComparison.CurrentCultureIgnoreCase) ? "Email" : "Phone";
        var isValidCode = await _userManager.VerifyTwoFactorTokenAsync(user, provider, request.CodigoVerificacao);
        
        if (!isValidCode)
        {
            await _mediator.Publish(new DomainNotification("ChangePassword", "Código de verificação inválido ou expirado."), cancellationToken);
            return ResponseBase<ChangePasswordCommandResponse>.Failure();
        }

        var result = await _userManager.ChangePasswordAsync(user, request.SenhaAtual, request.NovaSenha);

        if (result.Succeeded)
        {
            return ResponseBase<ChangePasswordCommandResponse>.Success(new ChangePasswordCommandResponse
            {
                Message = "Senha redefinida com sucesso."
            });
        }

        foreach (var error in result.Errors)
        {
            await _mediator.Publish(new DomainNotification("ChangePassword", error.Description), cancellationToken);
        }

        return ResponseBase<ChangePasswordCommandResponse>.Failure();
    }
}
