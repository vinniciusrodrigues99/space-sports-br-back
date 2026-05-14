using FSP.Api.Application.Common.Helpers;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Constants;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Application.Features.SpaceSports.Users.Commands.UpdateUserRole
{
    public class UpdateUserRoleCommandHandler(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        ILogService logService)
        : IRequestHandler<UpdateUserRoleCommand, ResponseBase<UserDTO>>
    {
        private static readonly HashSet<string> RolesValidos = ["admin", "author", "reader"];

        public async Task<ResponseBase<UserDTO>> Handle(UpdateUserRoleCommand command, CancellationToken cancellationToken)
        {
            if (!RolesValidos.Contains(command.Request.Role))
            {
                await mediator.Publish(new DomainNotification("UpdateRole", "Role inválido. Use: admin, author ou reader."), cancellationToken);
                return ResponseBase<UserDTO>.Failure();
            }

            var usuario = await userManager.FindByIdAsync(command.UserId.ToString());
            if (usuario == null)
            {
                await mediator.Publish(new DomainNotification("UpdateRole", "Usuário não encontrado."), cancellationToken);
                return ResponseBase<UserDTO>.Failure();
            }

            var novoRoleDb = RoleMapper.ToDbRole(command.Request.Role);

            var rolesAtuais = await userManager.GetRolesAsync(usuario);
            await userManager.RemoveFromRolesAsync(usuario, rolesAtuais);
            await userManager.AddToRoleAsync(usuario, novoRoleDb);

            // Reader nunca pode publicar
            if (novoRoleDb == ProfileRoles.Reader)
                usuario.CanPublish = false;

            var result = await userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                await mediator.Publish(new DomainNotification("UpdateRole", "Erro ao atualizar o role."), cancellationToken);
                return ResponseBase<UserDTO>.Failure();
            }

            await logService.Information(
                descricao: $"Role atualizado para {usuario.Email}",
                detalhes: System.Text.Json.JsonSerializer.Serialize(new
                {
                    UsuarioId = usuario.Id,
                    NovoRole = command.Request.Role,
                    Acao = "UpdateRole",
                    Modulo = "Usuarios"
                })
            );

            var dto = new UserDTO
            {
                Id = usuario.Id.ToString(),
                Name = usuario.NomeCompleto,
                Email = usuario.Email!,
                AvatarUrl = usuario.TxFoto,
                Role = command.Request.Role,
                CanPublish = usuario.CanPublish,
                CreatedAt = usuario.CriadoEm
            };

            return ResponseBase<UserDTO>.Success(dto);
        }
    }
}
