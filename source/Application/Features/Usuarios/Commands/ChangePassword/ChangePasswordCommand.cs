using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.Usuarios.Commands.ChangePassword;

public record ChangePasswordCommand(Guid IdUsuario, ChangePasswordRequest Request) : IRequest<ResponseBase<ChangePasswordCommandResponse>>;
