namespace FSP.Api.Application.Features.Autenticacao.Commands.RequestResetTokenPassword
{
    public record RequestResetTokenPasswordRequest
    {
        public required string Email { get; set; }
        //public required string Password { get; set; }

    }
}
