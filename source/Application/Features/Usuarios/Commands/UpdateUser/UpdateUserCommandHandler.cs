using Microsoft.AspNetCore.Identity;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;

namespace FSP.Api.Application.Features.Usuarios.Commands.UpdateUser
{
    public class UpdateUserCommandHandler(
        UserManager<ApplicationUser> _userManager, 
        IMediator _mediator,
        ICurrentUserService _userService,
        ILogService _logService) : IRequestHandler<UpdateUserCommand, ResponseBase<UpdateUserCommandResponse>>
    {
        public async Task<ResponseBase<UpdateUserCommandResponse>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var request = command.UpdateUserRequest;

            var user = await _userManager.FindByIdAsync(command.IdUsuario.ToString());
            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("UpdateUser", "Usuário não localizado."), cancellationToken);
                return ResponseBase<UpdateUserCommandResponse>.Failure();
            }

            // Capturar dados antigos para log de alterações
            var dadosAntigos = new
            {
                Email = user.Email,
                NomeCompleto = user.NomeCompleto,
                Celular = user.PhoneNumber,
                Perfil = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };

            var camposAlterados = new List<string>();

            if (request.Email != user.Email)
            {
                camposAlterados.Add($"Email: '{user.Email}' para '{request.Email}'");
                
                var emailExists = await _userManager.FindByEmailAsync(request.Email);
                if (emailExists != null)
                {
                    await _mediator.Publish(new DomainNotification("UpdateUser", "O email informado já está em uso."), cancellationToken);
                    return ResponseBase<UpdateUserCommandResponse>.Failure();
                }

                var setEmailResult = await _userManager.SetEmailAsync(user, request.Email);
                if (!setEmailResult.Succeeded)
                {
                    await _mediator.Publish(new DomainNotification("UpdateUser", "Falha ao atualizar o email do usuário."), cancellationToken);
                    return ResponseBase<UpdateUserCommandResponse>.Failure();
                }

                var setUserNameResult = await _userManager.SetUserNameAsync(user, request.Email);
                if (!setUserNameResult.Succeeded)
                {
                    await _mediator.Publish(new DomainNotification("UpdateUser", "Falha ao atualizar o nome de usuário."), cancellationToken);
                    return ResponseBase<UpdateUserCommandResponse>.Failure();
                }
            }

            var perfil = await _userManager.GetRolesAsync(user);

            if (perfil.Count > 0 && perfil[0] != request.Perfil.ToString())
            {
                camposAlterados.Add($"Perfil: '{perfil[0]}' para '{request.Perfil}'");
                await _userManager.RemoveFromRolesAsync(user, perfil);
                await _userManager.AddToRoleAsync(user, request.Perfil.ToString());
            }

            if (user.NomeCompleto != request.NomeCompleto)
                camposAlterados.Add($"Nome Completo: '{user.NomeCompleto}' para '{request.NomeCompleto}'");
            
            if (user.PhoneNumber != request.Celular)
                camposAlterados.Add($"Celular: '{user.PhoneNumber}' para '{request.Celular}'");

            user.NomeCompleto = request.NomeCompleto;
            user.PhoneNumber = request.Celular;
            
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Registrar log de atualização
                var descricaoMudancas = camposAlterados.Count > 0 
                    ? $"Campos alterados: {string.Join("; ", camposAlterados)}" 
                    : "Não houve alterações nos campos.";

                await _logService.Information(
                    descricao: $"Usuário '{user.NomeCompleto}' atualizado",
                    detalhes: System.Text.Json.JsonSerializer.Serialize(new
                    {
                        AlteradoPor = _userService.UserName ?? "Sistema",
                        DataAlteracao = DateTime.UtcNow,
                        UsuarioId = user.Id,
                        Email = user.Email,
                        NomeCompleto = user.NomeCompleto,
                        CamposAlterados = camposAlterados,
                        Acao = "Edicao",
                        Modulo = "Usuarios",
                        Entidade = "Usuario",
                        DadosAntigos = dadosAntigos,
                        DescricaoMudancas = descricaoMudancas
                    })
                );

                var response = new UpdateUserCommandResponse
                {
                    IdUsuario = user.Id,
                    NomeCompleto = user.NomeCompleto, 
                    Email = user.Email
                };

                return ResponseBase<UpdateUserCommandResponse>.Success(response);
            }

            await _mediator.Publish(new DomainNotification("UpdateUser", "Falha ao atualizar o usuário"), cancellationToken);
            return ResponseBase<UpdateUserCommandResponse>.Failure();
        }
    }
}
