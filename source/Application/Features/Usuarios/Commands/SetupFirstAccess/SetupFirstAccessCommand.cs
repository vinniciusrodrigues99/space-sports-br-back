using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.Usuarios.Commands.SetupFirstAccess;

public record SetupFirstAccessCommand(SetupFirstAccessRequest Request) : IRequest<ResponseBase<SetupFirstAccessCommandResponse>>;
