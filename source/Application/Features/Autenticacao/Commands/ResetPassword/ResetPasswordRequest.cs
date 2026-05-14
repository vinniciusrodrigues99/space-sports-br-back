namespace FSP.Api.Application.Features.Autenticacao.Commands.ResetPassword
{
    public record ResetPasswordRequest
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }
}
