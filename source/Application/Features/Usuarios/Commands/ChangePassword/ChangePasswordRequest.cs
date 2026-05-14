namespace FSP.Api.Application.Features.Usuarios.Commands.ChangePassword;

public record ChangePasswordRequest
{
    public required string SenhaAtual { get; set; }
    public required string NovaSenha { get; set; }
    public required string ConfirmarNovaSenha { get; set; }
    public required string CodigoVerificacao { get; set; }
    public required string MetodoEnvio { get; set; } // "email" ou "sms"
}
