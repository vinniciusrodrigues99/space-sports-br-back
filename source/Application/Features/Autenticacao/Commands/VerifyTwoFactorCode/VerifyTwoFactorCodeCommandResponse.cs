namespace FSP.Api.Application.Features.Autenticacao.Commands.VerifyTwoFactorCode;

public class VerifyTwoFactorCodeCommandResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public double ExpiraEm { get; set; }
}
