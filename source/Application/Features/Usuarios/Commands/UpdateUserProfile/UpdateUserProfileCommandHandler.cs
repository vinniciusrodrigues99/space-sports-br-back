using Microsoft.AspNetCore.Identity;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using FSP.Api.Application.Common.Interfaces;

namespace FSP.Api.Application.Features.Usuarios.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler(
    UserManager<ApplicationUser> _userManager,
    ICurrentUserService _currentUserService,
    IMediator _mediator) : IRequestHandler<UpdateUserProfileCommand, ResponseBase<UpdateUserProfileCommandResponse>>
{
    public async Task<ResponseBase<UpdateUserProfileCommandResponse>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var userAutenticated = _currentUserService;

        var user = await _userManager.FindByIdAsync(command.IdUsuario.ToString());
        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("UpdateUserProfile", "Usuário não localizado."), cancellationToken);
            return ResponseBase<UpdateUserProfileCommandResponse>.Failure();
        }

        if (!string.IsNullOrWhiteSpace(request.NomeCompleto))
            user.NomeCompleto = request.NomeCompleto;

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                await _mediator.Publish(new DomainNotification("UpdateUserProfile", "Erro ao atualizar o celular."), cancellationToken);
                return ResponseBase<UpdateUserProfileCommandResponse>.Failure();
            }
        }

        if (request.Telefone != null)
            user.Telefone = request.Telefone;

        if (request.TwoFactorEnabled.HasValue)
        {
            var setTwoFactorResult = await _userManager.SetTwoFactorEnabledAsync(user, request.TwoFactorEnabled.Value);
            if (!setTwoFactorResult.Succeeded)
            {
                await _mediator.Publish(new DomainNotification("UpdateUserProfile", "Erro ao atualizar autenticação de dois fatores."), cancellationToken);
                return ResponseBase<UpdateUserProfileCommandResponse>.Failure();
            }
        }

        if (request.NotificacaoEmail.HasValue)
            user.NotificacaoEmail = request.NotificacaoEmail.Value;

        if (request.NotificacaoSMS.HasValue)
            user.NotificacaoSMS = request.NotificacaoSMS.Value;

        user.DataModificacao = DateTimeOffset.UtcNow;
        user.ModificadoPor = userAutenticated.UserName;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            var response = new UpdateUserProfileCommandResponse
            {
                IdUsuario = user.Id,
                NomeCompleto = user.NomeCompleto,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Telefone = user.Telefone,
                TwoFactorEnabled = user.TwoFactorEnabled,
                NotificacaoEmail = user.NotificacaoEmail,
                NotificacaoSMS = user.NotificacaoSMS
            };

            return ResponseBase<UpdateUserProfileCommandResponse>.Success(response);
        }

        foreach (var error in result.Errors)
        {
            await _mediator.Publish(new DomainNotification("UpdateUserProfile", error.Description), cancellationToken);
        }

        return ResponseBase<UpdateUserProfileCommandResponse>.Failure();
    }
}
