using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSP.Api.Application.Features.SpaceSports.Posts.Queries.GetPosts;
using FSP.Api.Application.Features.SpaceSports.Posts.Queries.GetPostBySlug;
using FSP.Api.Application.Features.SpaceSports.Posts.Commands.CreatePost;
using FSP.Api.Application.Features.SpaceSports.Posts.Commands.UpdatePost;
using FSP.Api.Application.Features.SpaceSports.Posts.Commands.DeletePost;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;
using System.Security.Claims;

namespace FSP.Api.WebApi.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostsController(INotificationHandler<DomainNotification> notifications,
                                 INotificationHandler<DomainSuccesNotification> succesNotifications,
                                 IMediator mediatorHandler) : BaseController(notifications, succesNotifications, mediatorHandler)
    {
        private readonly IMediator _mediator = mediatorHandler;

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<List<PostDTO>>))]
        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] string? category)
            => Response(await _mediator.Send(new GetPostsQuery(category)));

        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<PostDTO>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetPostBySlug([FromRoute] string slug)
            => Response(await _mediator.Send(new GetPostBySlugQuery(slug)));

        [Authorize(Policy = "CanPublish")]
        [ProducesResponseType(statusCode: 201, type: typeof(ResponseBase<PostDTO>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new CreatePostCommand(request, userId));

            if (result.IsSuccess)
                return StatusCode(201, result);

            return Response(result);
        }

        [Authorize]
        [ProducesResponseType(statusCode: 200, type: typeof(ResponseBase<PostDTO>))]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePost([FromRoute] Guid id, [FromBody] UpdatePostRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Administrador");
            return Response(await _mediator.Send(new UpdatePostCommand(id, request, userId, isAdmin)));
        }

        [Authorize]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(statusCode: 400, type: typeof(ResponseBase<>))]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Administrador");
            var result = await _mediator.Send(new DeletePostCommand(id, userId, isAdmin));

            if (result.IsSuccess)
                return NoContent();

            return Response(result);
        }
    }
}
