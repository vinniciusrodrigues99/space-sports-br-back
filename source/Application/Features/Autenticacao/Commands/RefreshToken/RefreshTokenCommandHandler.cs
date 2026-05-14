using Microsoft.AspNetCore.Identity;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using System.Security.Claims;

namespace FSP.Api.Application.Features.Autenticacao.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(ITokenService _tokenService, UserManager<ApplicationUser> _userManager, IMediator _mediator) : IRequestHandler<RefreshTokenCommand, ResponseBase<RefreshTokenCommandResponse>>
    {
        public async Task<ResponseBase<RefreshTokenCommandResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var request = command.RefreshTokenRequest;


            var principal = _tokenService.GetPrincipal(request.AccessToken);
            var usuario = await _userManager.FindByEmailAsync(principal?.FindFirst(ClaimTypes.Email)?.Value);

            if (!await CommandBusinessValidators(usuario, request, cancellationToken))
                return ResponseBase<RefreshTokenCommandResponse>.Failure();

            var informacaoToken = await _tokenService.GenerateJWT(usuario, false);

            return ResponseBase<RefreshTokenCommandResponse>.Success(new RefreshTokenCommandResponse
            {
                Token = informacaoToken.Token,
                RefreshToken = informacaoToken.RefreshToken,
                ExpiraEm = informacaoToken.ExpiraEm,
            });
        }
        private async Task<bool> CommandBusinessValidators(ApplicationUser? usuario, RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            bool valid = true;

            if (usuario is null)
            {
                await _mediator.Publish(new DomainNotification("RefreshToken", "Não foi possível identificar o Usuário para o token informado."), cancellationToken);
                valid = false;
                return valid;
            }

            if (usuario.RefreshToken != request.RefreshToken)
            {
                await _mediator.Publish(new DomainNotification("RefreshToken", "O Refresh token informado é inválido."), cancellationToken);
                valid = false;
                return valid;
            }

            if (usuario.DataExpiracaoRefreshToken <= DateTime.Now)
            {
                await _mediator.Publish(new DomainNotification("RefreshToken", "O Refresh token informado esta expirado."), cancellationToken);
                valid = false;
                return valid;
            }

            return valid;
        }
    }
}
