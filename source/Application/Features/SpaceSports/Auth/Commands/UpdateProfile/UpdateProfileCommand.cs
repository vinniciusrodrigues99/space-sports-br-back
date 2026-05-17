using MediatR;
using FSP.Api.Domain.Common;
using FSP.Api.Application.Features.SpaceSports.DTOs;

namespace FSP.Api.Application.Features.SpaceSports.Auth.Commands.UpdateProfile;

public record UpdateProfileCommand(Guid UserId, string Name, string? AvatarUrl)
    : IRequest<ResponseBase<UserDTO>>;
