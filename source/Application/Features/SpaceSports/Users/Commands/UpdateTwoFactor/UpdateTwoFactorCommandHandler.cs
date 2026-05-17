using FSP.Api.Application.Common.Helpers;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Application.Features.SpaceSports.Users.Commands.UpdateTwoFactor;

public class UpdateTwoFactorCommandHandler(
    IMediator mediator,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<UpdateTwoFactorCommand, ResponseBase<UserDTO>>
{
    public async Task<ResponseBase<UserDTO>> Handle(UpdateTwoFactorCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
        {
            await mediator.Publish(new DomainNotification("UpdateTwoFactor", "Usuário não encontrado."), cancellationToken);
            return ResponseBase<UserDTO>.Failure();
        }

        var result = await userManager.SetTwoFactorEnabledAsync(user, command.Request.Enabled);
        if (!result.Succeeded)
        {
            await mediator.Publish(new DomainNotification("UpdateTwoFactor", "Erro ao atualizar 2FA."), cancellationToken);
            return ResponseBase<UserDTO>.Failure();
        }

        var roles = await userManager.GetRolesAsync(user);
        var roleDb = roles.FirstOrDefault() ?? "Leitor";

        return ResponseBase<UserDTO>.Success(new UserDTO
        {
            Id = user.Id.ToString(),
            Name = user.NomeCompleto,
            Email = user.Email!,
            AvatarUrl = user.TxFoto,
            Role = RoleMapper.ToFrontendRole(roleDb),
            CanPublish = user.CanPublish,
            TwoFactorEnabled = user.TwoFactorEnabled,
            CreatedAt = user.CriadoEm
        });
    }
}
