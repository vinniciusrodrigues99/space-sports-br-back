using Microsoft.AspNetCore.Identity;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using FSP.Api.Application.Common.Interfaces;

namespace FSP.Api.Application.Features.Autenticacao.Commands.RequestResetTokenPassword
{
    public class RequestResetTokenPasswordCommandHandler(IMediator _mediator, UserManager<ApplicationUser> _userManager, IEmailService emailService) : IRequestHandler<RequestResetTokenPasswordCommand, ResponseBase<RequestResetTokenPasswordCommandResponse>>
    {
        private readonly IEmailService _emailService = emailService;
        public async Task<ResponseBase<RequestResetTokenPasswordCommandResponse>> Handle(RequestResetTokenPasswordCommand command, CancellationToken cancellationToken)
        {
            var request = command.ResetTokenPasswordRequest;

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("RequestResetTokenPassword", "Usuário não localizado."), cancellationToken);
                return ResponseBase<RequestResetTokenPasswordCommandResponse>.Failure();
            }

            // var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            // if (!isPasswordValid)
            // {
            //     await _mediator.Publish(new DomainNotification("RequestResetTokenPassword", "Senha inválida."), cancellationToken);
            //     return ResponseBase<RequestResetTokenPasswordCommandResponse>.Failure();
            // }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailService.SendResetPasswordTokenAsync(user.Email!, token);

            return ResponseBase<RequestResetTokenPasswordCommandResponse>.Success(new RequestResetTokenPasswordCommandResponse
            {
                Mensagem = "Token de redefinição de senha enviado para o e-mail cadastrado."
            });
        }
    }
}
