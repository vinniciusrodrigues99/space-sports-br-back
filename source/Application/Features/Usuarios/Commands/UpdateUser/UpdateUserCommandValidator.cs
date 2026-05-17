using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSP.Api.Domain.Constants;

namespace FSP.Api.Application.Features.Usuarios.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.IdUsuario)
                .NotEmpty().WithMessage("O Id do usuário é obrigatório.");

            RuleFor(x => x.UpdateUserRequest.Email)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .EmailAddress().WithMessage("O email fornecido não é válido.");

            RuleFor(x => x.UpdateUserRequest.NomeCompleto)
                .NotEmpty().WithMessage("O nome completo é obrigatório.");

            RuleFor(x => x.UpdateUserRequest.CPF)
                .NotEmpty().WithMessage("O CPF é obrigatório.");

            RuleFor(x => x.UpdateUserRequest.Celular)
                .NotEmpty().WithMessage("O número de telefone é obrigatório.");

             RuleFor(x => x.UpdateUserRequest.Perfil)
                .NotEmpty().WithMessage("Perfil é obrigatório.")
                .Must(BeAValidRole).WithMessage("Perfil inválido. Valores válidos: Administrador, Autor, Leitor.");
        }

        private bool BeAValidRole(string perfil)
        {
            var validRoles = new[] { ProfileRoles.Admin, ProfileRoles.Author, ProfileRoles.Reader };
            return validRoles.Contains(perfil);
        }
    }
}