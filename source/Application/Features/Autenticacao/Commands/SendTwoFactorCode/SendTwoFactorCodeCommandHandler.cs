using Microsoft.AspNetCore.Identity;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Autenticacao.Commands.SendTwoFactorCode;

public class SendTwoFactorCodeCommandHandler(
    IMediator _mediator,
    UserManager<ApplicationUser> _userManager,
    IEmailService _emailService,
    ICacheService _cacheService,
    ISmsService _smsService) : IRequestHandler<SendTwoFactorCodeCommand, ResponseBase<SendTwoFactorCodeCommandResponse>>
{
    public async Task<ResponseBase<SendTwoFactorCodeCommandResponse>> Handle(SendTwoFactorCodeCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !user.TwoFactorEnabled)
        {
            await _mediator.Publish(new DomainNotification("SendTwoFactorCode", "Usuário não encontrado ou autenticação de dois fatores não está ativa."), cancellationToken);
            return ResponseBase<SendTwoFactorCodeCommandResponse>.Failure();
        }

        var lockKey = $"2fa:resend:lock:{user.Id}";
        var countKey = $"2fa:resend:count:{user.Id}";
        var isLocked = await _cacheService.ExistsAsync(lockKey);

        if (isLocked)
        {
            await _mediator.Publish(new DomainNotification("SendTwoFactorCode", "Limite de tentativas atingido. Tente novamente mais tarde."), cancellationToken);
            return ResponseBase<SendTwoFactorCodeCommandResponse>.Failure();
        }

        var currentCount = await _cacheService.GetCounterAsync(countKey);

        if (currentCount >= 3)
        {
            await _cacheService.SetLockAsync(lockKey, TimeSpan.FromMinutes(5));
            await _mediator.Publish(new DomainNotification("SendTwoFactorCode", "Limite de tentativas atingido. Tente novamente mais tarde."), cancellationToken);
            return ResponseBase<SendTwoFactorCodeCommandResponse>.Failure();
        }

        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("SendTwoFactorCode", "Usuário não encontrado."), cancellationToken);
            return ResponseBase<SendTwoFactorCodeCommandResponse>.Failure();
        }

        if (!user.TwoFactorEnabled)
        {
            await _mediator.Publish(new DomainNotification("SendTwoFactorCode", "Autenticação de dois fatores não está ativa para este usuário."), cancellationToken);
            return ResponseBase<SendTwoFactorCodeCommandResponse>.Failure();
        }

        var provider = request.MetodoEnvio.Equals("email", StringComparison.CurrentCultureIgnoreCase) ? "Email" : "Phone";
        var code = await _userManager.GenerateTwoFactorTokenAsync(user, provider);

        try
        {
            if (request.MetodoEnvio.Equals("email", StringComparison.CurrentCultureIgnoreCase))
            {
                await _emailService.SendTwoFactorCodeAsync(user.Email!, code);
            }

            else
            {
                await _smsService.SendTwoFactorCodeAsync(user.PhoneNumber!, code);
            }

            await _cacheService.IncrementCounterAsync(countKey, TimeSpan.FromMinutes(10));

            var expiresAt = DateTime.Now.AddMinutes(5);

            return ResponseBase<SendTwoFactorCodeCommandResponse>.Success(new SendTwoFactorCodeCommandResponse
            {
                Message = $"Código de verificação enviado para {(request.MetodoEnvio.ToLower() == "email" ? "seu email" : "seu telefone")}.",
                ExpiresAt = expiresAt
            });
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new DomainNotification("SendTwoFactorCode", $"Erro ao enviar código: {ex.Message}"), cancellationToken);
            return ResponseBase<SendTwoFactorCodeCommandResponse>.Failure();
        }
    }
}
