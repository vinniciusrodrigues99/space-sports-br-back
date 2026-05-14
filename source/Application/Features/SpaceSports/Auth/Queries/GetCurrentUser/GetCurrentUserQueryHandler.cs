using FSP.Api.Application.Common.Helpers;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Application.Features.SpaceSports.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler(
        IMediator mediator,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<GetCurrentUserQuery, ResponseBase<UserDTO>>
    {
        public async Task<ResponseBase<UserDTO>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var usuario = await userManager.FindByIdAsync(request.UserId.ToString());
            if (usuario == null)
            {
                await mediator.Publish(new DomainNotification("GetMe", "Usuário não encontrado."), cancellationToken);
                return ResponseBase<UserDTO>.Failure();
            }

            var roles = await userManager.GetRolesAsync(usuario);
            var roleDb = roles.FirstOrDefault() ?? "Leitor";

            var dto = new UserDTO
            {
                Id = usuario.Id.ToString(),
                Name = usuario.NomeCompleto,
                Email = usuario.Email!,
                AvatarUrl = usuario.TxFoto,
                Role = RoleMapper.ToFrontendRole(roleDb),
                CanPublish = usuario.CanPublish,
                CreatedAt = usuario.CriadoEm
            };

            return ResponseBase<UserDTO>.Success(dto);
        }
    }
}
