namespace FSP.Api.Application.Features.Usuarios.Commands.RequestChangePasswordCode;

public record RequestChangePasswordCodeCommandResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
