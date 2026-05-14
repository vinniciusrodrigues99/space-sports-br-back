using Microsoft.AspNetCore.Identity;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;
using System.Security.Cryptography;

namespace FSP.Api.Application.Features.Usuarios.Commands.CreateUser
{
    public class CreateUserCommandHandler(IMediator _mediator, 
                                          UserManager<ApplicationUser> _userManager,
                                          IEmailService _emailService, 
                                          ICurrentUserService _userService,
                                          ILogService _logService) : IRequestHandler<CreateUserCommand, ResponseBase<CreateUserCommandResponse>>
    {

        public async Task<ResponseBase<CreateUserCommandResponse>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var nomeUsuario = _userService.UserName;
            var request = command.CreateUserRequest;

            var novoUsuario = new ApplicationUser
            {
                UserName = request.Email,
                NomeCompleto = request.NomeCompleto,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber ?? string.Empty,
                CriadoPor = nomeUsuario,
                CriadoEm = DateTime.Now,
                DataModificacao = DateTime.Now,
                EmailConfirmed = false, // Conta não confirmada até primeiro acesso
                TwoFactorEnabled = true,
                CPF = request.Cpf,
                PrimeiroTokenAcesso = GenerateSecureToken(),
                ExpiracaoPrimeiroAcesso = DateTime.UtcNow.AddHours(48) 
            };

            return await CreateUserAsync(novoUsuario, request.Perfil, cancellationToken);
        }

        private async Task<ResponseBase<CreateUserCommandResponse>> CreateUserAsync(ApplicationUser novoUsuario, string perfil, CancellationToken cancellationToken)
        {
            var result = await _userManager.CreateAsync(novoUsuario);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(novoUsuario, perfil);
                
                if (!roleResult.Succeeded)
                {
                    await _mediator.Publish(new DomainNotification("CreateUser", $"Usuário criado, mas falha ao associar ao perfil '{perfil}'."), cancellationToken);
                    return ResponseBase<CreateUserCommandResponse>.Failure();
                }

                var frontendUrl = "https://seuapp.com"; // TODO: Mover para configuração
                var firstAccessLink = $"{frontendUrl}/primeiro-acesso?token={novoUsuario.PrimeiroTokenAcesso}";
                
                var emailBody = $@"
                    <h2>Bem-vindo ao Sistema!</h2>
                    <p>Olá, {novoUsuario.NomeCompleto}!</p>
                    <p>Sua conta foi criada com sucesso. Para ativar sua conta e definir sua senha, clique no link abaixo:</p>
                    <p><a href='{firstAccessLink}'>Definir Senha</a></p>
                    <p>Este link é válido por 48 horas.</p>
                    <p>Se você não solicitou esta conta, ignore este email.</p>
                ";

                await _emailService.SendEmailAsync(
                    novoUsuario.Email!,
                    "Ative sua conta",
                    emailBody
                );

                // Registrar log de criação de usuário
                await _logService.Information(
                    descricao: $"Usuário '{novoUsuario.NomeCompleto}' criado com sucesso",
                    detalhes: System.Text.Json.JsonSerializer.Serialize(new
                    {
                        CriadoPor = _userService.UserName ?? "Sistema",
                        DataCriacao = DateTime.UtcNow,
                        UsuarioId = novoUsuario.Id,
                        Email = novoUsuario.Email,
                        NomeCompleto = novoUsuario.NomeCompleto,
                        Perfil = perfil,
                        CPF = novoUsuario.CPF,
                        Telefone = novoUsuario.PhoneNumber,
                        Acao = "Criacao",
                        Modulo = "Usuarios",
                        Entidade = "Usuario"
                    })
                );

                return ResponseBase<CreateUserCommandResponse>.Success(new CreateUserCommandResponse
                {
                    IdUsuario = novoUsuario.Id,
                    Email = novoUsuario.Email,
                    NomeCompleto = novoUsuario.NomeCompleto,
                    Perfil = perfil
                });
            }

            await _mediator.Publish(new DomainNotification("CreateUser", "Falha ao criar o usuário."), cancellationToken);
            return ResponseBase<CreateUserCommandResponse>.Failure();
        }

        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
