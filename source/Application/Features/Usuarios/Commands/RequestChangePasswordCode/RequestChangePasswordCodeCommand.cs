using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.Usuarios.Commands.RequestChangePasswordCode;

public record RequestChangePasswordCodeCommand(Guid IdUsuario, RequestChangePasswordCodeRequest Request) : IRequest<ResponseBase<RequestChangePasswordCodeCommandResponse>>;
