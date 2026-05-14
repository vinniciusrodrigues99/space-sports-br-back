using FSP.Api.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;


namespace FSP.Api.Infrastructure.Services
{
    public class EmailService(ILogger<EmailService> logger, IConfiguration configuration) : IEmailService
    {
        private readonly ILogger<EmailService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public async Task SendTwoFactorCodeAsync(string email, string code)
        {
            try 
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["SmtpSettings:FromName"], 
                    _configuration["SmtpSettings:FromEmail"]!));

                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = "Seu código de verificação";
                
                var builder = new BodyBuilder
                {
                    HtmlBody = $"<p>Seu código de autenticação de dois fatores é: <strong>{code}</strong></p><p>Este código expira em 5 minutos.</p>"
                };
                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["SmtpSettings:Host"]!, 
                    int.Parse(_configuration["SmtpSettings:Port"]!), 
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _configuration["SmtpSettings:UserName"]!, 
                    _configuration["SmtpSettings:Password"]!);

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Enviando código 2FA {code} para o email {email}");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar código 2FA para {email}");
                throw;
            }            
        }

        public async Task SendResetPasswordTokenAsync(string email, string token)
        {
            try 
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["SmtpSettings:FromName"], 
                    _configuration["SmtpSettings:FromEmail"]!));

                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = "Token para Redefinição de Senha";
                
                var resetUrl = $"{_configuration["AppSettings:ResetPasswordUrl"]}?token={token}&email={Uri.EscapeDataString(email)}";
                var builder = new BodyBuilder
                {
                    HtmlBody = $"<p>Para redefinir sua senha, <a href='{resetUrl}'>clique aqui</a>."
                };
                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["SmtpSettings:Host"]!, 
                    int.Parse(_configuration["SmtpSettings:Port"]!), 
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _configuration["SmtpSettings:UserName"]!, 
                    _configuration["SmtpSettings:Password"]!);

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Enviando token de redefinição de senha para o email {email}");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar token de redefinição de senha para {email}");
                throw;
            }            
        }

        public async Task SendEmailAsync(string email, string subject, string htmlBody)
        {
            try 
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["SmtpSettings:FromName"], 
                    _configuration["SmtpSettings:FromEmail"]!));

                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;
                
                var builder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["SmtpSettings:Host"]!, 
                    int.Parse(_configuration["SmtpSettings:Port"]!), 
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _configuration["SmtpSettings:UserName"]!, 
                    _configuration["SmtpSettings:Password"]!);

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email '{subject}' enviado para {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar email '{subject}' para {email}");
                throw;
            }            
        }
    }
}
