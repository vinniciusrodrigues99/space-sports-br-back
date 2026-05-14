namespace FSP.Api.Application.Features.Autenticacao.Commands.Login
{
    public class LoginCommandResponse
    {
        public string? Token { get; set; }
        public double ExpiraEm { get; set; }
        public string? RefreshToken { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public string? Message { get; set; }
    }
}
