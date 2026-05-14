namespace FSP.Api.Application.Features.Autenticacao.Commands.SendTwoFactorCode;

public record SendTwoFactorCodeRequest
{
    public required string Email { get; set; }
    public required string MetodoEnvio { get; set; } // "Email" ou "SMS"
}
