using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Auth.Commands.Login
{
    public record SpaceSportsLoginCommand(SpaceSportsLoginRequest Request) : IRequest<ResponseBase<AuthSessionDTO>>;
}
