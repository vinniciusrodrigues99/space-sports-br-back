namespace FSP.Api.Application.Features.Usuarios.Commands.RequestChangePasswordCode;

public record RequestChangePasswordCodeRequest
{
    public required string MetodoEnvio { get; set; }
}
