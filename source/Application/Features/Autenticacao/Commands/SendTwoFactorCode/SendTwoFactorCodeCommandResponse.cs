namespace FSP.Api.Application.Features.Autenticacao.Commands.SendTwoFactorCode;

public class SendTwoFactorCodeCommandResponse
{
    public required string Message { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
