using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.Usuarios.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(Guid IdUsuario, UpdateUserProfileRequest Request) : IRequest<ResponseBase<UpdateUserProfileCommandResponse>>;
