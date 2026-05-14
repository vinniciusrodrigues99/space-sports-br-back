using Microsoft.AspNetCore.Identity;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;

namespace FSP.Api.Application.Features.Autenticacao.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ResetPasswordCommand, ResponseBase<ResetPasswordCommandResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ResponseBase<ResetPasswordCommandResponse>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var request = command.ResetPasswordRequest;

            var user = await _userManager.FindByEmailAsync(request.Email);
            
            if (user == null)
            {
                return ResponseBase<ResetPasswordCommandResponse>.Failure("E-mail inválido.");
            }

            var isValidToken = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", request.Token);
            
            if (!isValidToken)
            {
                return ResponseBase<ResetPasswordCommandResponse>.Failure("Token inválido.");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            
            if (!result.Succeeded)
            {
                var errorMessages = result.Errors.Select(e => e.Description).ToList();
                return ResponseBase<ResetPasswordCommandResponse>.Failure(result.Errors.FirstOrDefault()?.Description ?? "Falha ao redefinir a senha.");
            }

            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);

            return ResponseBase<ResetPasswordCommandResponse>.Success(new ResetPasswordCommandResponse
            {
                Success = true,
                Message = "Redefinição de senha bem-sucedida."
            });
        }
    }
}
