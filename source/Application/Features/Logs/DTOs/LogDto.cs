namespace FSP.Api.Application.Features.Logs.DTOs;

public class LogDto
{
    public string EnderecoIp { get; set; } = string.Empty;
    public string AgenteUsuario { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
    public string NomeUsuario { get; set; } = string.Empty;
    public DateTimeOffset?  DataHora { get; set; }
    public string Detalhes { get; set; } = string.Empty;
}