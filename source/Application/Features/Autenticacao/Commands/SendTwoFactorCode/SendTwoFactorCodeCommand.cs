using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Autenticacao.Commands.SendTwoFactorCode;

public class SendTwoFactorCodeCommand(SendTwoFactorCodeRequest request) : Command<SendTwoFactorCodeCommandResponse>
{
    public SendTwoFactorCodeRequest Request { get; set; } = request;
}

