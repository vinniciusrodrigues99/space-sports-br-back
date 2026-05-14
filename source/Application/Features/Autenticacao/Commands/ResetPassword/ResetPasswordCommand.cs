using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Autenticacao.Commands.ResetPassword
{
    public class ResetPasswordCommand(ResetPasswordRequest resetPasswordRequest) : Command<ResetPasswordCommandResponse>
    {
        public ResetPasswordRequest ResetPasswordRequest { get; set; } = resetPasswordRequest;
    }
}
