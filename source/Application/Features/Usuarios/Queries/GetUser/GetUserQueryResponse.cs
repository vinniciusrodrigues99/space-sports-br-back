

namespace FSP.Api.Application.Features.Usuarios.Queries.GetUser
{
    public class GetUserQueryResponse
    {
        public required Guid IdUsuario { get; set; }
        public string? NomeCompleto { get; set; }
        public string? Email { get; set; }
    }
}
