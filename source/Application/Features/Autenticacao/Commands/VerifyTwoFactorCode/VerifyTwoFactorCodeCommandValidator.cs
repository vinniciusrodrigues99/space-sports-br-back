namespace FSP.Api.Application.Features.Autenticacao.Commands.VerifyTwoFactorCode;

public class VerifyTwoFactorCodeCommandValidator : AbstractValidator<VerifyTwoFactorCodeCommand>
{
    public VerifyTwoFactorCodeCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email inválido.");

        RuleFor(x => x.Request.TwoFactorCode)
            .NotEmpty().WithMessage("Código é obrigatório.")
            .Length(6).WithMessage("Código deve ter 6 dígitos.")
            .Matches("^[0-9]{6}$").WithMessage("Código deve conter apenas números.");
    }
}
