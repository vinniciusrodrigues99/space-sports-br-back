using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace FSP.Api.Application.Features.Usuarios.Commands.ResendFirstAccess;

public class ResendFirstAccessCommandHandler(
    UserManager<ApplicationUser> _userManager,
    IEmailService _emailService,
    IMediator _mediator) : IRequestHandler<ResendFirstAccessCommand, ResponseBase<ResendFirstAcessCommandResponse>>
{
    public async Task<ResponseBase<ResendFirstAcessCommandResponse>> Handle(ResendFirstAccessCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.IdUsuario && !u.Excluido, cancellationToken);

        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("ResendFirstAccess", "Usuário não encontrado."), cancellationToken);
            return ResponseBase<ResendFirstAcessCommandResponse>.Failure();
        }

        if (user.EmailConfirmed)
        {
            await _mediator.Publish(new DomainNotification("ResendFirstAccess", "Esta conta já está ativada. Não é necessário reenviar o link de primeiro acesso."), cancellationToken);
            return ResponseBase<ResendFirstAcessCommandResponse>.Failure();
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (hasPassword)
        {
            await _mediator.Publish(new DomainNotification("ResendFirstAccess", "Este usuário já possui senha. Use o fluxo de reset de senha."), cancellationToken);
            return ResponseBase<ResendFirstAcessCommandResponse>.Failure();
        }

        user.PrimeiroTokenAcesso = GenerateSecureToken();
        user.ExpiracaoPrimeiroAcesso = DateTime.UtcNow.AddHours(48); 

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            await _mediator.Publish(new DomainNotification("ResendFirstAccess", $"Erro ao gerar novo token: {errors}"), cancellationToken);
            return ResponseBase<ResendFirstAcessCommandResponse>.Failure();
        }

        var frontendUrl = "https://seuapp.com"; 
        var firstAccessLink = $"{frontendUrl}/primeiro-acesso?token={user.PrimeiroTokenAcesso}";

        var emailBody = $@"
            <h2>Novo Link de Ativação</h2>
            <p>Olá, {user.NomeCompleto}!</p>
            <p>Você solicitou um novo link para ativar sua conta e definir sua senha.</p>
            <p>Clique no link abaixo para ativar sua conta:</p>
            <p><a href='{firstAccessLink}'>Definir Senha</a></p>
            <p>Este link é válido por 48 horas.</p>
            <p>Se você não solicitou esta ativação, ignore este email.</p>
        ";

        await _emailService.SendEmailAsync(
            user.Email!,
            "Novo Link de Ativação de Conta",
            emailBody
        );

        await _mediator.Publish(new DomainSuccesNotification("ResendFirstAccess", "Novo link de primeiro acesso enviado com sucesso!"), cancellationToken);

        return ResponseBase<ResendFirstAcessCommandResponse>.Success(
            new ResendFirstAcessCommandResponse($"Um novo link de ativação foi enviado para o email {user.Email}. O link é válido por 48 horas.")
        );
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}
