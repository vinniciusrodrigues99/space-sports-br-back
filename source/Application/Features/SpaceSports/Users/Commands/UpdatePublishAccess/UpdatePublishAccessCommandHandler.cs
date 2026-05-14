using FSP.Api.Application.Common.Helpers;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Application.Features.SpaceSports.Users.Commands.UpdatePublishAccess
{
    public class UpdatePublishAccessCommandHandler(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        ILogService logService)
        : IRequestHandler<UpdatePublishAccessCommand, ResponseBase<UserDTO>>
    {
        public async Task<ResponseBase<UserDTO>> Handle(UpdatePublishAccessCommand command, CancellationToken cancellationToken)
        {
            var usuario = await userManager.FindByIdAsync(command.UserId.ToString());
            if (usuario == null)
            {
                await mediator.Publish(new DomainNotification("UpdatePublishAccess", "Usuário não encontrado."), cancellationToken);
                return ResponseBase<UserDTO>.Failure();
            }

            var roles = await userManager.GetRolesAsync(usuario);
            var roleDb = roles.FirstOrDefault() ?? "Leitor";

            // Reader nunca pode publicar
            if (roleDb == "Leitor" && command.Request.CanPublish)
            {
                await mediator.Publish(new DomainNotification("UpdatePublishAccess", "Usuários com role 'reader' não podem ter permissão de publicar."), cancellationToken);
                return ResponseBase<UserDTO>.Failure();
            }

            usuario.CanPublish = command.Request.CanPublish;
            var result = await userManager.UpdateAsync(usuario);

            if (!result.Succeeded)
            {
                await mediator.Publish(new DomainNotification("UpdatePublishAccess", "Erro ao atualizar permissão."), cancellationToken);
                return ResponseBase<UserDTO>.Failure();
            }

            await logService.Information(
                descricao: $"Permissão de publicar atualizada para {usuario.Email}",
                detalhes: System.Text.Json.JsonSerializer.Serialize(new
                {
                    UsuarioId = usuario.Id,
                    CanPublish = command.Request.CanPublish,
                    Acao = "UpdatePublishAccess",
                    Modulo = "Usuarios"
                })
            );

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
