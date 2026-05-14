using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Features.RoleManagement.Commands.CreateRole;
using FSP.Api.Domain.Notifications;
using FSP.Api.Domain.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FSP.Api.Domain.Entities.Permission;
using FSP.Api.Domain.Entities.Usuario;

namespace FSP.Api.Application.Features.RoleManagement.Commands.CreateRole
{
    public class CreateRoleCommandHandler(
        RoleManager<Perfil> roleManager,
        IMediator mediator,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateRoleCommand, ResponseBase<CreateRoleCommandResponse>>
    {
        private readonly RoleManager<Perfil> _roleManager = roleManager;
        private readonly IMediator _mediator = mediator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResponseBase<CreateRoleCommandResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            // Check if role already exists
            var existingRole = await _roleManager.FindByNameAsync(request.CreateRoleRequest.RoleName);
            if (existingRole != null)
            {
                await _mediator.Publish(new DomainNotification("ValidationError", "Já existe uma role com este nome."), cancellationToken);
                return ResponseBase<CreateRoleCommandResponse>.Failure();
            }

            // Validate permissions using database
            var validPermissionNames = await _unitOfWork.GetRepository<Permissao>()
                .AsQueryable()
                .Where(p => !p.Excluido)
                .Select(p => p.Nome)
                .ToListAsync(cancellationToken);
            
            var validPermissions = validPermissionNames.ToHashSet();

            foreach (var permission in request.CreateRoleRequest.Permissions)
            {
                if (!validPermissions.Contains(permission))
                {
                    await _mediator.Publish(new DomainNotification("ValidationError", $"Permissão inválida: {permission}"), cancellationToken);
                    return ResponseBase<CreateRoleCommandResponse>.Failure();
                }
            }

            // Create role
            var role = new Perfil{ Name = request.CreateRoleRequest.RoleName };
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                await _mediator.Publish(new DomainNotification("ValidationError", $"Erro ao criar role: {string.Join(", ", result.Errors.Select(e => e.Description))}"), cancellationToken);
                return ResponseBase<CreateRoleCommandResponse>.Failure();
            }

            // Add permissions as claims
            foreach (var permission in request.CreateRoleRequest.Permissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }

            return ResponseBase<CreateRoleCommandResponse>.Success(new CreateRoleCommandResponse
            {
                RoleName = request.CreateRoleRequest.RoleName
            });
        }
    }
}
