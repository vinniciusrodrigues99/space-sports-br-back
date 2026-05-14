namespace FSP.Api.Application.Features.Usuarios.Commands.UploadProfilePhoto;

public record UploadProfilePhotoCommandResponse
{
    public string NomeArquivo { get; set; } = string.Empty;
    public string CaminhoCompleto { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
