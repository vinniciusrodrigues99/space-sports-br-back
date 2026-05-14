namespace FSP.Api.Application.Features.Usuarios.Commands.UpdateUserProfile;

public record UpdateUserProfileRequest
{
    public string? NomeCompleto { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Telefone { get; set; }
    public bool? TwoFactorEnabled { get; set; }
    public bool? NotificacaoEmail { get; set; }
    public bool? NotificacaoSMS { get; set; }
}
