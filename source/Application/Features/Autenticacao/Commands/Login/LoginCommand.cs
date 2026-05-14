using FSP.Api.Application.Features.Autenticacao.Commands.Login;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Autenticacao.Commands.Login
{
    public class LoginCommand(LoginRequest loginRequest) : Command<LoginCommandResponse>
    {
        public LoginRequest LoginRequest { get; set; } = loginRequest;
    }
}
