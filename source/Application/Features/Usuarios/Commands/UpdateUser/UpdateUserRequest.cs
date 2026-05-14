using FSP.Api.Domain.Constants;
using FSP.Api.Domain.Enums;

namespace FSP.Api.Application.Features.Usuarios.Commands.UpdateUser
{
    public record UpdateUserRequest
    {
        public required string Email { get; set; }
        public required string NomeCompleto { get; set; }
        public required string Celular { get; set; }
        public required string CPF { get; set; }
        public required string Perfil { get; set; }
    }
}
