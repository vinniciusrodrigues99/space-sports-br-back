namespace FSP.Api.Application.Features.Usuarios.Commands.UpdateUserProfile;

public record UpdateUserProfileCommandResponse
{
    public Guid IdUsuario { get; set; }
    public string? NomeCompleto { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Telefone { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool NotificacaoEmail { get; set; }
    public bool NotificacaoSMS { get; set; }
}
