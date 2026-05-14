using FSP.Api.Application.Common.Helpers;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Application.Features.SpaceSports.Auth.Commands.Login
{
    public class SpaceSportsLoginCommandHandler(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogService logService)
        : IRequestHandler<SpaceSportsLoginCommand, ResponseBase<AuthSessionDTO>>
    {
        public async Task<ResponseBase<AuthSessionDTO>> Handle(SpaceSportsLoginCommand command, CancellationToken cancellationToken)
        {
            var req = command.Request;

            var usuario = await userManager.FindByEmailAsync(req.Email);
            if (usuario == null)
            {
                await mediator.Publish(new DomainNotification("Login", "Usuário e/ou senha incorreto(s)."), cancellationToken);
                return ResponseBase<AuthSessionDTO>.Failure();
            }

            var resultado = await signInManager.CheckPasswordSignInAsync(usuario, req.Password, true);

            if (resultado.IsLockedOut)
            {
                await mediator.Publish(new DomainNotification("Login", "Conta bloqueada devido a múltiplas tentativas falhas."), cancellationToken);
                return ResponseBase<AuthSessionDTO>.Failure();
            }

            if (!resultado.Succeeded)
            {
                await mediator.Publish(new DomainNotification("Login", "Usuário e/ou senha incorreto(s)."), cancellationToken);
                return ResponseBase<AuthSessionDTO>.Failure();
            }

            var roles = await userManager.GetRolesAsync(usuario);
            var roleDb = roles.FirstOrDefault() ?? "Leitor";
            var roleFrontend = RoleMapper.ToFrontendRole(roleDb);

            var tokenResponse = await tokenService.GenerateJWT(usuario, true);

            await logService.Information(
                descricao: $"Login realizado: {usuario.Email}",
                detalhes: System.Text.Json.JsonSerializer.Serialize(new
                {
                    UsuarioId = usuario.Id,
                    Email = usuario.Email,
                    Acao = "Login",
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
                    Role = roleFrontend,
                    CanPublish = usuario.CanPublish,
                    CreatedAt = usuario.CriadoEm
                }
            };

            return ResponseBase<AuthSessionDTO>.Success(dto);
        }
    }
}
