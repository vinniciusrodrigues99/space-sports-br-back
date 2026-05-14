namespace FSP.Api.Application.Features.Autenticacao.Commands.SendTwoFactorCode;

public class SendTwoFactorCodeCommandValidator : AbstractValidator<SendTwoFactorCodeCommand>
{
    public SendTwoFactorCodeCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.");

        RuleFor(x => x.Request.MetodoEnvio)
            .NotEmpty().WithMessage("Método de envio é obrigatório.")
            .Must(BeAValidMethod).WithMessage("Método de envio inválido. Valores válidos: Email, SMS.");
    }

    private bool BeAValidMethod(string metodo)
    {
        var validMethods = new[] { "Email", "SMS", "email", "sms" };
        return validMethods.Contains(metodo);
    }
}
