using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FSP.Api.Application.Features.Usuarios.Commands.SetupFirstAccess;

public class SetupFirstAccessCommandHandler(
    UserManager<ApplicationUser> _userManager,
    IMediator _mediator) : IRequestHandler<SetupFirstAccessCommand, ResponseBase<SetupFirstAccessCommandResponse>>
{
    public async Task<ResponseBase<SetupFirstAccessCommandResponse>> Handle(SetupFirstAccessCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PrimeiroTokenAcesso == request.Request.Token && u.ExpiracaoPrimeiroAcesso > DateTime.UtcNow, cancellationToken);

        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("SetupFirstAccess", "Token inválido ou expirado."), cancellationToken);
            return ResponseBase<SetupFirstAccessCommandResponse>.Failure();
        }

        if (user.EmailConfirmed)
        {
            await _mediator.Publish(new DomainNotification("SetupFirstAccess", "Esta conta já foi ativada anteriormente."), cancellationToken);
            return ResponseBase<SetupFirstAccessCommandResponse>.Failure();
        }

        var result = await _userManager.AddPasswordAsync(user, request.Request.Senha);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            await _mediator.Publish(new DomainNotification("SetupFirstAccess", $"Erro ao definir senha: {errors}"), cancellationToken);
            return ResponseBase<SetupFirstAccessCommandResponse>.Failure();
        }

        user.EmailConfirmed = true;
        user.PrimeiroTokenAcesso = null;
        user.ExpiracaoPrimeiroAcesso = null;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            await _mediator.Publish(new DomainNotification("SetupFirstAccess", $"Erro ao ativar conta: {errors}"), cancellationToken);
            return ResponseBase<SetupFirstAccessCommandResponse>.Failure();
        }

        await _mediator.Publish(new DomainSuccesNotification("SetupFirstAccess", "Senha definida com sucesso! Sua conta foi ativada."), cancellationToken);
        
        return ResponseBase<SetupFirstAccessCommandResponse>.Success(
            new SetupFirstAccessCommandResponse("Conta ativada com sucesso. Você já pode fazer login no sistema.")
        );
    }
}
