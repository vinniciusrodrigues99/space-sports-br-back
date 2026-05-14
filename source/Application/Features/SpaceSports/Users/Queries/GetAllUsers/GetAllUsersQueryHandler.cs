using FSP.Api.Application.Common.Helpers;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FSP.Api.Application.Features.SpaceSports.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager)
        : IRequestHandler<GetAllUsersQuery, ResponseBase<List<UserDTO>>>
    {
        public async Task<ResponseBase<List<UserDTO>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var usuarios = await userManager.Users
                .Where(u => !u.Excluido)
                .OrderBy(u => u.NomeCompleto)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var dtos = new List<UserDTO>();

            foreach (var usuario in usuarios)
            {
                var roles = await userManager.GetRolesAsync(usuario);
                var roleDb = roles.FirstOrDefault() ?? "Leitor";

                dtos.Add(new UserDTO
                {
                    Id = usuario.Id.ToString(),
                    Name = usuario.NomeCompleto,
                    Email = usuario.Email!,
                    AvatarUrl = usuario.TxFoto,
                    Role = RoleMapper.ToFrontendRole(roleDb),
                    CanPublish = usuario.CanPublish,
                    CreatedAt = usuario.CriadoEm
                });
            }

            return ResponseBase<List<UserDTO>>.Success(dtos);
        }
    }
}
