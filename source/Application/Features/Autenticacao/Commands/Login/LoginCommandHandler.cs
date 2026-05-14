using Microsoft.AspNetCore.Identity;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Interfaces;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Autenticacao.Commands.Login
{
    public class LoginCommandHandler(IMediator _mediator,
                                     SignInManager<ApplicationUser> _loginManager,
                                     UserManager<ApplicationUser> _userManager, 
                                     ITokenService _tokenService,
                                     IEmailService _emailService,
                                     ISmsService _smsService,
                                     ILogService _logService) : IRequestHandler<LoginCommand, ResponseBase<LoginCommandResponse>>
    {

        public async Task<ResponseBase<LoginCommandResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var request = command.LoginRequest;

            var usuario = await _userManager.FindByEmailAsync(request.Email);

            if (usuario == null)
            {
                await _logService.Warning(
                    descricao: $"Tentativa de login com email inexistente: {request.Email}",
                    detalhes: System.Text.Json.JsonSerializer.Serialize(new
                    {
                        Email = request.Email,
                        DataTentativa = DateTime.UtcNow,
                        Acao = "LoginFalha",
                        Motivo = "Email não encontrado",
                        Modulo = "Autenticacao"
                    })
                );
                await _mediator.Publish(new DomainNotification("Login", "Usuário e/ou senha incorreto(s)."), cancellationToken);
                return ResponseBase<LoginCommandResponse>.Failure();
            }

            var resultSignin = await _loginManager.CheckPasswordSignInAsync(usuario, request.Password, true);
            
            if (resultSignin.IsLockedOut)
            {
                await _logService.Warning(
                    descricao: $"Conta bloqueada por tentativas inválidas: {usuario.Email}",
                    detalhes: System.Text.Json.JsonSerializer.Serialize(new
                    {
                        UsuarioId = usuario.Id,
                        Email = usuario.Email,
                        DataTentativa = DateTime.UtcNow,
                        Acao = "LoginFalha",
                        Motivo = "Conta bloqueada",
                        Modulo = "Autenticacao"
                    })
                );
                await _mediator.Publish(new DomainNotification("Login", "Conta bloqueada devido a múltiplas tentativas de login falhas. Redefina a senha."), cancellationToken);
                return ResponseBase<LoginCommandResponse>.Failure();
            }

            if (!resultSignin.Succeeded && !resultSignin.RequiresTwoFactor)
            {
                await _logService.Warning(
                    descricao: $"Tentativa de login com senha incorreta: {usuario.Email}",
                    detalhes: System.Text.Json.JsonSerializer.Serialize(new
                    {
                        UsuarioId = usuario.Id,
                        Email = usuario.Email,
                        DataTentativa = DateTime.UtcNow,
                        Acao = "LoginFalha",
                        Motivo = "Senha incorreta",
                        Modulo = "Autenticacao"
                    })
                );
                await _mediator.Publish(new DomainNotification("Login", "Usuário e/ou senha incorreto(s)."), cancellationToken);
                return ResponseBase<LoginCommandResponse>.Failure();
            }

            if(usuario.TwoFactorEnabled)
            {
                await _logService.Information(
                    descricao: $"Login com 2FA iniciado para: {usuario.Email}",
                    detalhes: System.Text.Json.JsonSerializer.Serialize(new
                    {
                        UsuarioId = usuario.Id,
                        Email = usuario.Email,
                        DataTentativa = DateTime.UtcNow,
                        Acao = "Login2FA",
                        Modulo = "Autenticacao"
                    })
                );
                var provider = !string.IsNullOrWhiteSpace(usuario.Email) ? "Email" : "Phone"; 
                var code = await _userManager.GenerateTwoFactorTokenAsync(usuario, provider);

                try
                {
                    if (provider == "Email")
                    {
                        await _emailService.SendTwoFactorCodeAsync(usuario.Email!, code);
                    }
                    else
                    {
                        await _smsService.SendTwoFactorCodeAsync(usuario.PhoneNumber!, code);
                    }

                    return ResponseBase<LoginCommandResponse>.Success(new LoginCommandResponse
                    {
                        RequiresTwoFactor = true,
                        Message = $"Código de verificação enviado para {(provider == "Email" ? "seu email" : "seu telefone")}."
                    });
                }
                catch (Exception ex)
                {
                    await _logService.Error(
                        descricao: $"Erro ao enviar código de autenticação 2FA para: {usuario.Email}",
                        detalhes: ex.Message,
                        exception: ex
                    );
                    await _mediator.Publish(new DomainNotification("Login", $"Erro ao enviar código de autenticação: {ex.Message}"), cancellationToken);
                    return ResponseBase<LoginCommandResponse>.Failure();
                }
            }

            var informacoesToken = await _tokenService.GenerateJWT(usuario, true, request.RememberMe);

            // Registrar log de login bem-sucedido
            await _logService.Information(
                descricao: $"Login realizado com sucesso por '{usuario.NomeCompleto}'",
                detalhes: System.Text.Json.JsonSerializer.Serialize(new
                {
                    UsuarioId = usuario.Id,
                    Email = usuario.Email,
                    NomeCompleto = usuario.NomeCompleto,
                    DataLogin = DateTime.UtcNow,
                    RememberMe = request.RememberMe,
                    TwoFactorEnabled = usuario.TwoFactorEnabled,
                    Acao = "Login",
                    Modulo = "Autenticacao",
                    Entidade = "Usuario"
                })
            );

            return ResponseBase<LoginCommandResponse>.Success(new LoginCommandResponse
            {
                Token = informacoesToken.Token,
                ExpiraEm = informacoesToken.ExpiraEm,
                RefreshToken = informacoesToken.RefreshToken,
                RequiresTwoFactor = false
            });
        }
    }
}
