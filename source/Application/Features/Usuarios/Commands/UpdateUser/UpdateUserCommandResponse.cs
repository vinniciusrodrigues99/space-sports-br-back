

namespace FSP.Api.Application.Features.Usuarios.Commands.UpdateUser
{
    public class UpdateUserCommandResponse
    {
        public required Guid IdUsuario { get; set; }
        public string? NomeCompleto { get; set; }
        public string? Email { get; set; }
    }
}
