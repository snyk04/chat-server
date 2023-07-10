using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using chat_server.AuthModule.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace chat_server.AuthModule;

public class TokenManager : ITokenManager
{
    private record TokenPayload(ClaimsPrincipal Principal, SecurityToken SecurityToken);

    /// <summary>
    /// Returns principal from auth token
    /// </summary>
    /// <param name="token">User auth token</param>
    /// <param name="issuerSigningKey">Issuer signing key</param>
    /// <returns>ClaimsPrincipal</returns>
    /// <exception cref="SecurityTokenException">Thrown if something is wrong with token</exception>
    public ClaimsPrincipal GetPrincipalFromToken(string token, string issuerSigningKey)
    {
        var tokenValidationParameters = GenerateTokenValidationParameters(issuerSigningKey);
        var tokenPayload = GetTokenPayload(token, tokenValidationParameters);
        
        if (!IsSecurityTokenValid(tokenPayload.SecurityToken))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return tokenPayload.Principal;
    }

    private TokenValidationParameters GenerateTokenValidationParameters(string issuerSigningKey)
    {
        // TODO : Configure it
        return new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey)),
            ValidateLifetime = true
        };
    }

    private TokenPayload GetTokenPayload(string token, TokenValidationParameters tokenValidationParameters)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        return new TokenPayload(principal, securityToken);
    }

    private static bool IsSecurityTokenValid(SecurityToken securityToken)
    {
        return securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg.Equals(
            SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
    }
}