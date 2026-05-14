namespace FSP.Api.Application.Common.Interfaces
{
    public interface ISmsService
    {
        Task SendTwoFactorCodeAsync(string phoneNumber, string code);
    }
}
