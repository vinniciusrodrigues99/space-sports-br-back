namespace FSP.Api.Application.Features.Usuarios.Commands.CreateUser
{
    public class CreateUserCommandResponse
    {
        public required Guid IdUsuario { get; set; }
        public string? NomeCompleto { get; set; }
        public string? Email { get; set; }
        public string? Perfil { get; set; }
    }
}
