using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;
using FSP.Api.Application.Features.SpaceSports.DTOs;

namespace FSP.Api.Application.Features.SpaceSports.Auth.Commands.Register
{
    public record RegisterCommand(RegisterRequest Request) : IRequest<ResponseBase<AuthSessionDTO>>;
}
