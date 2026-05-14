namespace FSP.Api.Application.Features.Usuarios.Commands.SetupFirstAccess;

public record SetupFirstAccessRequest
{
    public required string Token { get; set; }
    public required string Senha { get; set; }
    public required string ConfirmarSenha { get; set; }
}
