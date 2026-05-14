using FSP.Api.Domain.Constants;

namespace FSP.Api.Application.Features.Usuarios.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.CreateUserRequest.NomeCompleto)
                .NotEmpty().WithMessage("Nome completo é obrigatório.")
                .MaximumLength(200).WithMessage("Nome completo deve ter no máximo 200 caracteres.");

            RuleFor(x => x.CreateUserRequest.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório.")
                .EmailAddress().WithMessage("E-mail inválido.")
                .MaximumLength(100).WithMessage("E-mail deve ter no máximo 100 caracteres.");

            RuleFor(x => x.CreateUserRequest.Cpf)
                .NotEmpty().WithMessage("CPF é obrigatório.")
                .MaximumLength(14).WithMessage("CPF deve ter no máximo 14 caracteres.");

            RuleFor(x => x.CreateUserRequest.Perfil)
                .NotEmpty().WithMessage("Perfil é obrigatório.")
                .Must(BeAValidRole).WithMessage("Perfil inválido. Valores válidos: Administrador, Diretoria, Aprovador.");
        }

        private bool BeAValidRole(string perfil)
        {
            var validRoles = new[] { ProfileRoles.Admin, ProfileRoles.Board, ProfileRoles.Approver };
            return validRoles.Contains(perfil);
        }
    }
}
