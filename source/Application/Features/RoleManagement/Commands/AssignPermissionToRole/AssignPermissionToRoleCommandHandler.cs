using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Features.RoleManagement.Commands.AssignPermissionToRole;
using FSP.Api.Domain.Notifications;
using FSP.Api.Domain.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FSP.Api.Domain.Entities.Permission;
using FSP.Api.Domain.Entities.Usuario;

namespace FSP.Api.Application.Features.RoleManagement.Commands.AssignPermissionToRole
{
    public class AssignPermissionToRoleCommandHandler(
        RoleManager<Perfil> roleManager,
        IMediator mediator,
        IUnitOfWork unitOfWork) : IRequestHandler<AssignPermissionToRoleCommand, ResponseBase<AssignPermissionToRoleCommandResponse>>
    {
        private readonly RoleManager<Perfil> _roleManager = roleManager;
        private readonly IMediator _mediator = mediator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResponseBase<AssignPermissionToRoleCommandResponse>> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            // Get role
            var role = await _roleManager.FindByNameAsync(request.RoleName);
            
            if (role == null)
            {
                await _mediator.Publish(new DomainNotification("ValidationError", "Role não encontrada."), cancellationToken);
                return ResponseBase<AssignPermissionToRoleCommandResponse>.Failure();
            }

            // Validate permission using database
            var validPermissionNames = await _unitOfWork.GetRepository<Permissao>()
                .AsQueryable()
                .Where(p => !p.Excluido)
                .Select(p => p.Nome)
                .ToListAsync(cancellationToken);
            
            var validPermissions = validPermissionNames.ToHashSet();

            if (!validPermissions.Contains(request.AssignPermissionToRoleRequest.Permission))
            {
                await _mediator.Publish(new DomainNotification("ValidationError", $"Permissão inválida: {request.AssignPermissionToRoleRequest.Permission}"), cancellationToken);
                return ResponseBase<AssignPermissionToRoleCommandResponse>.Failure();
            }

            // Check if permission already exists
            var existingClaims = await _roleManager.GetClaimsAsync(role);
            if (existingClaims.Any(c => c.Type == "Permission" && c.Value == request.AssignPermissionToRoleRequest.Permission))
            {
                await _mediator.Publish(new DomainNotification("ValidationError", "A role já possui esta permissão."), cancellationToken);
                return ResponseBase<AssignPermissionToRoleCommandResponse>.Failure();
            }

            // Add permission as claim
            await _roleManager.AddClaimAsync(role, new Claim("Permission", request.AssignPermissionToRoleRequest.Permission));

            return ResponseBase<AssignPermissionToRoleCommandResponse>.Success(new AssignPermissionToRoleCommandResponse());
        }
    }
}
