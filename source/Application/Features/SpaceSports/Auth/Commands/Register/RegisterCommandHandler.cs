using FSP.Api.Application.Common.Helpers;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Constants;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Application.Features.SpaceSports.Auth.Commands.Register
{
    public class RegisterCommandHandler(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ILogService logService)
        : IRequestHandler<RegisterCommand, ResponseBase<AuthSessionDTO>>
    {
        public async Task<ResponseBase<AuthSessionDTO>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            var req = command.Request;

            var existente = await userManager.FindByEmailAsync(req.Email);
            if (existente != null)
            {
                await mediator.Publish(new DomainNotification("Register", "E-mail já cadastrado."), cancellationToken);
                return ResponseBase<AuthSessionDTO>.Failure();
            }

            var usuario = new ApplicationUser
            {
                UserName = req.Email,
                Email = req.Email,
                NomeCompleto = req.Name,
                EmailConfirmed = true,
                TwoFactorEnabled = false,
                CanPublish = false,
                CriadoPor = "register",
                CriadoEm = DateTimeOffset.UtcNow,
                DataModificacao = DateTimeOffset.UtcNow
            };

            var result = await userManager.CreateAsync(usuario, req.Password);
            if (!result.Succeeded)
            {
                var erros = string.Join(", ", result.Errors.Select(e => e.Description));
                await mediator.Publish(new DomainNotification("Register", erros), cancellationToken);
                return ResponseBase<AuthSessionDTO>.Failure();
            }

            await userManager.AddToRoleAsync(usuario, ProfileRoles.Reader);

            var tokenResponse = await tokenService.GenerateJWT(usuario, true);

            await logService.Information(
                descricao: $"Novo usuário registrado: {usuario.Email}",
                detalhes: System.Text.Json.JsonSerializer.Serialize(new
                {
                    UsuarioId = usuario.Id,
                    Email = usuario.Email,
                    Acao = "Register",
                    Modulo = "Autenticacao"
                })
            );

            var dto = new AuthSessionDTO
            {
                Token = tokenResponse.Token,
                User = new UserDTO
                {
                    Id = usuario.Id.ToString(),
                    Name = usuario.NomeCompleto,
                    Email = usuario.Email!,
                    AvatarUrl = usuario.TxFoto,
                    Role = "reader",
                    CanPublish = false,
                    CreatedAt = usuario.CriadoEm
                }
            };

            return ResponseBase<AuthSessionDTO>.Success(dto);
        }
    }
}
