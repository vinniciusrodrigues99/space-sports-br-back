using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using FSP.Api.Domain.Entities.Usuario;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Features.Autenticacao.Commands.Login.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FSP.Api.Infrastructure.Services
{
    public class TokenService(
        UserManager<ApplicationUser> _userManager, 
        RoleManager<Perfil> _roleManager,
        IConfiguration _configuration) : ITokenService
    {
        public async Task<TokenResponseDTO> GenerateJWT(ApplicationUser user, bool popularExp, bool rememberMe = false)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);

            var identityClaims = await GenerateClaims(userClaims, user);
            var encodedToken = TokenEncode(identityClaims);

            var refreshToken = GetRefreshToken();

            user.RefreshToken = refreshToken;

            if (popularExp)
            {
                // Se "Lembrar de mim" estiver marcado, o refresh token expira em 30 dias
                // Caso contrário, expira em 7 dias (padrão)
                user.DataExpiracaoRefreshToken = rememberMe 
                    ? DateTime.Now.AddDays(30) 
                    : DateTime.Now.AddDays(7);
            }

            await _userManager.UpdateAsync(user);

            return GetToken(encodedToken, refreshToken, user, userClaims);
        }

        public ClaimsPrincipal? GetPrincipal(string token)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:SecurityKey"] ?? "default_security_key"));
            var signingKeys = new List<SecurityKey> { signingKey };

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuers = [_configuration["JWTSettings:ValidIssuer"]],
                ValidAudiences = [_configuration["JWTSettings:ValidAudience"]],
                IssuerSigningKeys = signingKeys
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken is null
                || (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
                && !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase)))
                return null;

            return principal;
        }

        private async Task<ClaimsIdentity> GenerateClaims(ICollection<Claim> userClaims, ApplicationUser user)
        {
            //Claims do usuário
            var userRoles = await _userManager.GetRolesAsync(user);

            userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            
            // Adicionar canPublish claim
            userClaims.Add(new Claim("canPublish", user.CanPublish.ToString().ToLower()));

            // Adicionar roles
            foreach (var role in userRoles)
            {
                userClaims.Add(new Claim("role", role));

                // Adicionar as claims/permissions do role
                var perfil = await _roleManager.FindByNameAsync(role);
                if (perfil != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(perfil);
                    foreach (var claim in roleClaims)
                    {
                        if (!userClaims.Any(c => c.Type == "Permission" && c.Value == claim.Value))
                        {
                            userClaims.Add(new Claim("Permission", claim.Value));
                        }
                    }
                }
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(userClaims);
            return identityClaims;
        }

        private string TokenEncode(ClaimsIdentity claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWTSettings:SecurityKey"] ?? string.Empty);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWTSettings:ValidIssuer"] ?? string.Empty,
                Audience = _configuration["JWTSettings:ValidAudience"] ?? string.Empty,
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(int.Parse(_configuration["JWTSettings:ExpiryInHours"] ?? "48")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            });

            return tokenHandler.WriteToken(token);
        }

        private TokenResponseDTO GetToken(string encodedToken, string refreshToken, ApplicationUser user, IEnumerable<Claim> userClaims)
        {
            return new TokenResponseDTO
            {
                Token = encodedToken,
                ExpiraEm = TimeSpan.FromHours(int.Parse(_configuration["JWTSettings:ExpiryInHours"] ?? "48")).TotalSeconds,
                RefreshToken = refreshToken,
                InformacoesToken = new()
                {
                    Id = user.Id,
                    NomeUsuario = user.NomeCompleto,
                    Email = user.Email,
                    Claims = userClaims.Select(c => new TokenClaimDTO() { Tipo = c.Type, Valor = c.Value })
                }
            };
        }

        private static string GetRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - DateTimeOffset.UnixEpoch).TotalSeconds);
    }
}
