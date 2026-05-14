using Microsoft.AspNetCore.Identity;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using FSP.Api.Application.Features.Usuarios.Commands.DeleteUser;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;

namespace FSP.Api.Application.Features.Usuarios.Commands.DeleteUser
{
    public class DeleteUserCommandHandler(
        UserManager<ApplicationUser> _userManager, 
        IMediator _mediator,
        ICurrentUserService _userService,
        ILogService _logService) : IRequestHandler<DeleteUserCommand, ResponseBase<Unit>>
    {
        public async Task<ResponseBase<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserID.ToString());

            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("DeleteUser", "Usuário não localizado."), cancellationToken);
                return ResponseBase<Unit>.Failure();
            }

            var perfis = await _userManager.GetRolesAsync(user);

            await _userManager.DeleteAsync(user);

            // Registrar log de exclusão
            await _logService.Warning(
                descricao: $"Usuário '{user.NomeCompleto}' excluído",
                detalhes: System.Text.Json.JsonSerializer.Serialize(new
                {
                    ExcluidoPor = _userService.UserName ?? "Sistema",
                    DataExclusao = DateTime.UtcNow,
                    UsuarioId = user.Id,
                    Email = user.Email,
                    NomeCompleto = user.NomeCompleto,
                    CPF = user.CPF,
                    Perfis = perfis,
                    Acao = "Exclusao",
                    Modulo = "Usuarios",
                    Entidade = "Usuario"
                })
            );

            return ResponseBase<Unit>.Success(Unit.Value);
        }
    }
}
