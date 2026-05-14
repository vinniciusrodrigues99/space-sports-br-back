using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Features.RoleManagement.Queries.GetRoles;
using FSP.Api.Application.Features.RoleManagement.Queries.GetRoles.DTOs;
using FSP.Api.Domain.Entities.Usuario;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.RoleManagement.Queries.GetRoles
{
    public class GetRolesQueryHandler(RoleManager<Perfil> roleManager) : IRequestHandler<GetRolesQuery, ResponseBase<GetRolesQueryResponse>>
    {
        private readonly RoleManager<Perfil> _roleManager = roleManager;

        public async Task<ResponseBase<GetRolesQueryResponse>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
            var roleDtos = new List<RoleDto>();

            foreach (var role in roles)
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
            }

            return ResponseBase<GetRolesQueryResponse>.Success(new GetRolesQueryResponse
            {
                Roles = roleDtos
            });
        }
    }
}
