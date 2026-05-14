using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Usuarios.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    UserManager<ApplicationUser> _userManager,
    IConfiguration _configuration,
    IMediator _mediator) : IRequestHandler<GetUserProfileQuery, ResponseBase<GetUserProfileQueryResponse>>
{
    public async Task<ResponseBase<GetUserProfileQueryResponse>> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(query.IdUsuario.ToString());
        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("GetUserProfile", "Usuário não localizado."), cancellationToken);
            return ResponseBase<GetUserProfileQueryResponse>.Failure();
        }

        string? fotoBase64 = null;
        string? fotoContentType = null;

        if (!string.IsNullOrEmpty(user.TxFoto))
        {
            try
            {
                var profilePhotosPath = _configuration["FileStorage:ProfilePhotosPath"] 
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads", "profiles");

                var fullPath = Path.Combine(profilePhotosPath, user.TxFoto);

                if (File.Exists(fullPath))
                {
                    var bytes = await File.ReadAllBytesAsync(fullPath, cancellationToken);
                    fotoBase64 = Convert.ToBase64String(bytes);
                    var extension = Path.GetExtension(user.TxFoto).ToLower();
                    fotoContentType = extension switch
                    {
                        ".jpg" or ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".webp" => "image/webp",
                        _ => "application/octet-stream"
                    };
                }
            }
            catch
            {
                fotoBase64 = null;
                fotoContentType = null;
            }
        }

        var response = new GetUserProfileQueryResponse
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            Email = user.Email ?? string.Empty,
            CPF = user.CPF,
            PhoneNumber = user.PhoneNumber,
            Telefone = user.Telefone,
            FotoBase64 = fotoBase64,
            FotoContentType = fotoContentType,
            TwoFactorEnabled = user.TwoFactorEnabled,
            NotificacaoEmail = user.NotificacaoEmail,
            NotificacaoSMS = user.NotificacaoSMS
        };

        return ResponseBase<GetUserProfileQueryResponse>.Success(response);
    }
}
