using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Application.Features.RoleManagement.Commands.AssignRoleToUser
{
    public class AssignRoleToUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<Perfil> roleManager,
        IUser currentUser,
        IMediator mediator) : IRequestHandler<AssignRoleToUserCommand, ResponseBase<AssignRoleToUserCommandResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<Perfil> _roleManager = roleManager;
        private readonly IUser _currentUser = currentUser;
        private readonly IMediator _mediator = mediator;

        public async Task<ResponseBase<AssignRoleToUserCommandResponse>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.Id;
            Guard.Against.NullOrEmpty(currentUserId, nameof(currentUserId));

            // Check if current user has permission to assign roles
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser!);
            
            if (!currentUserRoles.Contains("Shipowner") && !currentUserRoles.Contains("Admin"))
            {
                await _mediator.Publish(new DomainNotification("ForbiddenAccess", "Apenas Shipowners e Admins podem atribuir roles."), cancellationToken);
                return ResponseBase<AssignRoleToUserCommandResponse>.Failure();
            }

            // Get target user
            var targetUser = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (targetUser == null)
            {
                await _mediator.Publish(new DomainNotification("ValidationError", "Usuário não encontrado."), cancellationToken);
                return ResponseBase<AssignRoleToUserCommandResponse>.Failure();
            }

            // Check if role exists
            var role = await _roleManager.FindByNameAsync(request.AssignRoleToUserRequest.RoleName);
            if (role == null)
            {
                await _mediator.Publish(new DomainNotification("ValidationError", "Role não encontrada."), cancellationToken);
                return ResponseBase<AssignRoleToUserCommandResponse>.Failure();
            }

            // Remove existing roles and assign new role
            var existingRoles = await _userManager.GetRolesAsync(targetUser);
            await _userManager.RemoveFromRolesAsync(targetUser, existingRoles);
            await _userManager.AddToRoleAsync(targetUser, request.AssignRoleToUserRequest.RoleName);

            return ResponseBase<AssignRoleToUserCommandResponse>.Success(new AssignRoleToUserCommandResponse());
        }
    }
}
