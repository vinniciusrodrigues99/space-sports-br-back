using FSP.Api.Application.Features.Autenticacao.Commands.Login.DTOs;
using FSP.Api.Domain.Entities.Usuario;
using System.Security.Claims;

namespace FSP.Api.Application.Common.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponseDTO> GenerateJWT(ApplicationUser user, bool pupularExp, bool rememberMe = false);
        ClaimsPrincipal? GetPrincipal(string token);
    }
}
