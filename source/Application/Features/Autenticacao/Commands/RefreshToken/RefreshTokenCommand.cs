using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Autenticacao.Commands.RefreshToken
{
    public class RefreshTokenCommand(RefreshTokenRequest refreshTokenRequest) : Command<RefreshTokenCommandResponse>
    {
        public RefreshTokenRequest RefreshTokenRequest { get; set; } = refreshTokenRequest;
    }
}
