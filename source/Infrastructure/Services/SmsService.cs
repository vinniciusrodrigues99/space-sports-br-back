using FSP.Api.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace FSP.Api.Infrastructure.Services
{
    public class SmsService(ILogger<SmsService> logger) : ISmsService
    {
        private readonly ILogger<SmsService> _logger = logger;

        public async Task SendTwoFactorCodeAsync(string phoneNumber, string code)
        {
            // TODO: Implementar integração com serviço de SMS (Twilio, AWS SNS, etc.)
            _logger.LogInformation($"Enviando código 2FA {code} para o telefone {phoneNumber}");
            
            // Simulação de envio
            await Task.Delay(100);
            
            // EXEMPLO DE IMPLEMENTAÇÃO COM TWILIO:
            // var accountSid = _configuration["Twilio:AccountSid"];
            // var authToken = _configuration["Twilio:AuthToken"];
            // TwilioClient.Init(accountSid, authToken);
            // var message = await MessageResource.CreateAsync(
            //     body: $"Seu código de verificação é: {code}. Válido por 5 minutos.",
            //     from: new PhoneNumber(_configuration["Twilio:PhoneNumber"]),
            //     to: new PhoneNumber(phoneNumber)
            // );
            
            _logger.LogInformation($"Código 2FA enviado com sucesso para {phoneNumber}");
        }
    }
}
