using Microsoft.AspNetCore.Identity;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Autenticacao.Commands.VerifyTwoFactorCode;

public class VerifyTwoFactorCodeCommandHandler(
    IMediator _mediator,
    UserManager<ApplicationUser> _userManager,
    ITokenService _tokenService) : IRequestHandler<VerifyTwoFactorCodeCommand, ResponseBase<VerifyTwoFactorCodeCommandResponse>>
{
    public async Task<ResponseBase<VerifyTwoFactorCodeCommandResponse>> Handle(VerifyTwoFactorCodeCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("VerifyTwoFactorCode", "Usuário não encontrado."), cancellationToken);
            return ResponseBase<VerifyTwoFactorCodeCommandResponse>.Failure();
        }

        if (!user.TwoFactorEnabled)
        {
            await _mediator.Publish(new DomainNotification("VerifyTwoFactorCode", "Autenticação de dois fatores não está ativa para este usuário."), cancellationToken);
            return ResponseBase<VerifyTwoFactorCodeCommandResponse>.Failure();
        }

        var provider = !string.IsNullOrWhiteSpace(user.Email) ? "Email" : "Phone";

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, provider, request.TwoFactorCode);

        if (!isValid)
        {
            await _mediator.Publish(new DomainNotification("VerifyTwoFactorCode", "Código inválido ou expirado. Verifique o código e tente novamente ou solicite um novo envio."), cancellationToken);
            return ResponseBase<VerifyTwoFactorCodeCommandResponse>.Failure();
        }

        var informacoesToken = await _tokenService.GenerateJWT(user, true);

        return ResponseBase<VerifyTwoFactorCodeCommandResponse>.Success(new VerifyTwoFactorCodeCommandResponse
        {
            Token = informacoesToken.Token,
            ExpiraEm = informacoesToken.ExpiraEm,
            RefreshToken = informacoesToken.RefreshToken!
        });
    }
}
