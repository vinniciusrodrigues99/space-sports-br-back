using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.SpaceSports.Users.Commands.UpdatePublishAccess
{
    public record UpdatePublishAccessCommand(Guid UserId, UpdatePublishAccessRequest Request) : IRequest<ResponseBase<UserDTO>>;
}
