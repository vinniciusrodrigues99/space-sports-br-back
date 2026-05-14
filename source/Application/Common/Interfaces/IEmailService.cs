namespace FSP.Api.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendTwoFactorCodeAsync(string email, string code);
        Task SendResetPasswordTokenAsync(string email, string token);
        Task SendEmailAsync(string email, string subject, string htmlBody);
    }
}
