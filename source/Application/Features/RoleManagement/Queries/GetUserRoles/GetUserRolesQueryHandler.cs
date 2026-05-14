using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Features.RoleManagement.Queries.GetUserRoles;
using FSP.Api.Application.Features.RoleManagement.Queries.GetRoles.DTOs;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using FSP.Api.Domain.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FSP.Api.Application.Features.RoleManagement.Queries.GetUserRoles
{
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, ResponseBase<GetUserRolesQueryResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<Perfil> _roleManager;
        private readonly IMediator _mediator;

        public GetUserRolesQueryHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<Perfil> roleManager,
            IMediator mediator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mediator = mediator;
        }

        public async Task<ResponseBase<GetUserRolesQueryResponse>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("ValidationError", "Usuário não encontrado."), cancellationToken);
                return ResponseBase<GetUserRolesQueryResponse>.Failure();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var roleDtos = new List<RoleDto>();
            var allPermissions = new List<string>();

            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    var permissions = claims
                        .Where(c => c.Type == "Permission")
                        .Select(c => c.Value)
                        .ToList();

                    roleDtos.Add(new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name!,
                        NormalizedName = role.NormalizedName,
                        Permissions = permissions
                    });

                    allPermissions.AddRange(permissions);
                }
            }

            return ResponseBase<GetUserRolesQueryResponse>.Success(new GetUserRolesQueryResponse
            {
                UserId = request.UserId,
                Roles = roleDtos,
                Permissions = allPermissions.Distinct().ToList()
            });
        }
    }
}
