using FSP.Api.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace FSP.Api.Application.Features.Usuarios.Commands.UploadProfilePhoto;

public record UploadProfilePhotoCommand(Guid IdUsuario, IFormFile Foto) : IRequest<ResponseBase<UploadProfilePhotoCommandResponse>>;
