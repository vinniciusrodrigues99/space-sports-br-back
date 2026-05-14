using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Usuarios.Commands.ResendFirstAccess
{
    public record ResendFirstAccessCommand(Guid IdUsuario) : IRequest<ResponseBase<ResendFirstAcessCommandResponse>>;
}