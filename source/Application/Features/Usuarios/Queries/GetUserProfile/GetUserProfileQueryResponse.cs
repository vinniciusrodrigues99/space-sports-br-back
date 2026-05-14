namespace FSP.Api.Application.Features.Usuarios.Queries.GetUserProfile;

public record GetUserProfileQueryResponse
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CPF { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Telefone { get; set; }
    public string? FotoBase64 { get; set; }
    public string? FotoContentType { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool NotificacaoEmail { get; set; }
    public bool NotificacaoSMS { get; set; }
}
