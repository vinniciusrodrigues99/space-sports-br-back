using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSP.Api.Application.Features.SpaceSports.Users.Queries.GetAllUsers;
using FSP.Api.Application.Features.SpaceSports.Users.Commands.UpdatePublishAccess;
using FSP.Api.Application.Features.SpaceSports.Users.Commands.UpdateUserRole;
using FSP.Api.Application.Features.SpaceSports.Users.Commands.UpdateTwoFactor;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.WebApi.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/users")]
    public class UsersAdminController(INotificationHandler<DomainNotification> notifications,
                          INotificationHandler<DomainSuccesNotification> succesNotifications,
                          IMediator mediator) : BaseController(notifications, succesNotifications, mediator)
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<List<UserDTO>>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [ProducesResponseType(statusCode: 403, type: typeof(ResponseBase<>))]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
            => Response(await _mediator.Send(new GetAllUsersQuery()));

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<UserDTO>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [ProducesResponseType(statusCode: 403, type: typeof(ResponseBase<>))]
        [HttpPatch("{id}/publish-access")]
        public async Task<IActionResult> UpdatePublishAccess(Guid id, [FromBody] UpdatePublishAccessRequest request)
            => Response(await _mediator.Send(new UpdatePublishAccessCommand(id, request)));

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<UserDTO>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [ProducesResponseType(statusCode: 403, type: typeof(ResponseBase<>))]
        [HttpPatch("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleRequest request)
            => Response(await _mediator.Send(new UpdateUserRoleCommand(id, request)));

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<UserDTO>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [ProducesResponseType(statusCode: 403, type: typeof(ResponseBase<>))]
        [HttpPatch("{id}/two-factor")]
        public async Task<IActionResult> UpdateTwoFactor(Guid id, [FromBody] UpdateTwoFactorRequest request)
            => Response(await _mediator.Send(new UpdateTwoFactorCommand(id, request)));
    }
}
