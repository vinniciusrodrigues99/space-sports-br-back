using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSP.Api.Application.Features.SpaceSports.Auth.Commands.Login;
using FSP.Api.Application.Features.SpaceSports.Auth.Commands.Register;
using FSP.Api.Application.Features.SpaceSports.Auth.Commands.UpdateProfile;
using FSP.Api.Application.Features.SpaceSports.Auth.Queries.GetCurrentUser;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;
using System.Security.Claims;

namespace FSP.Api.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class SpaceSportsAuthController(INotificationHandler<DomainNotification> notifications,
                                           INotificationHandler<DomainSuccesNotification> succesNotifications,
                                           IMediator mediatorHandler) : BaseController(notifications, succesNotifications, mediatorHandler)
    {
        private readonly IMediator _mediator = mediatorHandler;

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<AuthSessionDTO>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SpaceSportsLoginRequest request)
            => Response(await _mediator.Send(new SpaceSportsLoginCommand(request)));

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<AuthSessionDTO>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
            => Response(await _mediator.Send(new RegisterCommand(request)));

        [Authorize]
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<UserDTO>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userIdStr = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            return Response(await _mediator.Send(new GetCurrentUserQuery(userId)));
        }

        [Authorize]
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<UserDTO>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userIdStr = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            return Response(await _mediator.Send(new UpdateProfileCommand(userId, request.Name, request.AvatarUrl)));
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
            => NoContent();
    }
}
