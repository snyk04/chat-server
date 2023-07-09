using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using chat_server.AuthModule.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace chat_server.AuthModule;

public class TokenManager : ITokenManager
{
    /// <summary>
    /// Returns principal from auth token
    /// </summary>
    /// <param name="token">User auth token</param>
    /// <param name="issuerSigningKey">Issuer signing key</param>
    /// <returns>ClaimsPrincipal</returns>
    /// <exception cref="SecurityTokenException">Thrown if something is wrong with token</exception>
    public ClaimsPrincipal GetPrincipalFromToken(string token, string issuerSigningKey)
    {
        // TODO : Configure it
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey)),
            ValidateLifetime = true
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (!IsSecurityTokenValid(securityToken))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    private static bool IsSecurityTokenValid(SecurityToken securityToken)
    {
        return securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg.Equals(
            SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
    }
}