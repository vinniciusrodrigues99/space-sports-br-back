using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Usuarios.Commands.UploadProfilePhoto;

public class UploadProfilePhotoCommandHandler(
    UserManager<ApplicationUser> _userManager,
    IConfiguration _configuration,
    IMediator _mediator) : IRequestHandler<UploadProfilePhotoCommand, ResponseBase<UploadProfilePhotoCommandResponse>>
{
    public async Task<ResponseBase<UploadProfilePhotoCommandResponse>> Handle(UploadProfilePhotoCommand command, CancellationToken cancellationToken)
    {
        var foto = command.Foto;

        var user = await _userManager.FindByIdAsync(command.IdUsuario.ToString());
        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("UploadProfilePhoto", "Usuário não localizado."), cancellationToken);
            return ResponseBase<UploadProfilePhotoCommandResponse>.Failure();
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var maxFileSizeMB = 5;
        
        var extension = Path.GetExtension(foto.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
        {
            await _mediator.Publish(new DomainNotification("UploadProfilePhoto", $"Formato inválido. Formatos aceitos: {string.Join(", ", allowedExtensions)}"), cancellationToken);
            return ResponseBase<UploadProfilePhotoCommandResponse>.Failure();
        }

        if (foto.Length > maxFileSizeMB * 1024 * 1024)
        {
            await _mediator.Publish(new DomainNotification("UploadProfilePhoto", $"Tamanho máximo permitido: {maxFileSizeMB}MB."), cancellationToken);
            return ResponseBase<UploadProfilePhotoCommandResponse>.Failure();
        }

        try
        {
            var profilePhotosPath = _configuration["FileStorage:ProfilePhotosPath"] 
                ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads", "profiles");

            if (!Directory.Exists(profilePhotosPath))
            {
                Directory.CreateDirectory(profilePhotosPath);
            }

            if (!string.IsNullOrEmpty(user.TxFoto))
            {
                var oldFilePath = Path.Combine(profilePhotosPath, user.TxFoto);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            var fileName = $"{user.Id}{extension}";
            var fullPath = Path.Combine(profilePhotosPath, fileName);

            // 6. Salva arquivo
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await foto.CopyToAsync(stream, cancellationToken);
            }

            user.TxFoto = fileName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                await _mediator.Publish(new DomainNotification("UploadProfilePhoto", "Erro ao atualizar foto do usuário."), cancellationToken);
                return ResponseBase<UploadProfilePhotoCommandResponse>.Failure();
            }

            return ResponseBase<UploadProfilePhotoCommandResponse>.Success(new UploadProfilePhotoCommandResponse
            {
                NomeArquivo = fileName,
                CaminhoCompleto = fullPath,
                Message = "Foto de perfil atualizada com sucesso."
            });
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new DomainNotification("UploadProfilePhoto", $"Erro ao salvar foto: {ex.Message}"), cancellationToken);
            return ResponseBase<UploadProfilePhotoCommandResponse>.Failure();
        }
    }
}
