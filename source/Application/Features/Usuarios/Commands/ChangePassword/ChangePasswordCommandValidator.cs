namespace FSP.Api.Application.Features.Usuarios.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.Request.SenhaAtual)
            .NotEmpty().WithMessage("Senha atual é obrigatória.");

        RuleFor(x => x.Request.NovaSenha)
            .NotEmpty().WithMessage("Nova senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número.")
            .Matches(@"[!@#$%]").WithMessage("A senha deve conter pelo menos um caractere especial (! @ # $ %).");

        RuleFor(x => x.Request.ConfirmarNovaSenha)
            .NotEmpty().WithMessage("Confirmação de senha é obrigatória.")
            .Equal(x => x.Request.NovaSenha).WithMessage("As senhas não coincidem.");

        RuleFor(x => x.Request.CodigoVerificacao)
            .NotEmpty().WithMessage("Código de verificação é obrigatório.");

        RuleFor(x => x.Request.MetodoEnvio)
            .NotEmpty().WithMessage("Método de envio é obrigatório.")
            .Must(x => x.Equals("email", StringComparison.OrdinalIgnoreCase) || x.Equals("sms", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Método de envio deve ser 'email' ou 'sms'.");
    }
}
