using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Autenticacao.Commands.RequestResetTokenPassword
{
    public class RequestResetTokenPasswordCommand : Command<RequestResetTokenPasswordCommandResponse>
    {
        public RequestResetTokenPasswordRequest ResetTokenPasswordRequest { get; set; }

        public RequestResetTokenPasswordCommand(RequestResetTokenPasswordRequest resetTokenPasswordRequest)
        {
            ResetTokenPasswordRequest = resetTokenPasswordRequest;
        }
    }
}
