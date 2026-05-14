namespace FSP.Api.Application.Features.Autenticacao.Commands.VerifyTwoFactorCode;

public record VerifyTwoFactorCodeRequest
{
    public required string Email { get; set; }
    public required string TwoFactorCode { get; set; }
}
