using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using MediatR;

namespace FSP.Api.Application.Features.SpaceSports.Users.Commands.UpdateTwoFactor;

public record UpdateTwoFactorCommand(Guid UserId, UpdateTwoFactorRequest Request)
    : IRequest<ResponseBase<UserDTO>>;
