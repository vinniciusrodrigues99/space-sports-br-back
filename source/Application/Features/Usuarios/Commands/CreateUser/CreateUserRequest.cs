namespace FSP.Api.Application.Features.Usuarios.Commands.CreateUser
{
    public record CreateUserRequest
    {
        public required string NomeCompleto { get; set; }
        public required string Cpf { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Perfil { get; set; }
    }
}
