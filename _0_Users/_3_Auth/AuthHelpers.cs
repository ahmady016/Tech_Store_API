using System.Text;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

using Entities;

namespace Auth;

public class AccessTokenResponse
{
    public string Value { get; set; }
    public DateTime ExpiresAt { get; set; }
}
public class RefreshTokenResponse
{
    public string Value { get; set; }
    public DateTime ExpiresAt { get; set; }
}
public class TokensResponse
{
    public AccessTokenResponse AccessToken { get; set; }
    public RefreshTokenResponse RefreshToken { get; set; }
}

public static class AuthHelpers
{
    private static IConfiguration _config;
    public static void Initialize(IConfiguration Configuration)
    {
        _config = Configuration;
    }

    private static string TokenIssuer => _config["JWT:Issuer"];
    private static string TokenAudience => _config["JWT:Audience"];
    private static byte[] TokenSecret => Encoding.UTF8.GetBytes(_config["JWT:Secret"]);
    private static int AccessTokenValidityInMinutes => int.Parse(_config["JWT:AccessTokenValidityInMinutes"]);
    private static int RefreshTokenValidityInDays => int.Parse(_config["JWT:RefreshTokenValidityInDays"]);

    public static TokenValidationParameters GetTokenValidationOptions(bool validateLifetime)
    {
        return new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(TokenSecret),
            ValidIssuer = TokenIssuer,
            ValidAudience = TokenAudience,
            ValidateLifetime = validateLifetime,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
        };
    }
    public static List<Claim> ValidateTokenAndGetClaims(string token)
    {
        var tokenValidationParameters = GetTokenValidationOptions(validateLifetime: true);
        var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal.Claims.ToList();
    }

    public static AccessTokenResponse GenerateAccessToken(List<Claim> claims)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = TokenIssuer,
            Audience = TokenAudience,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(TokenSecret), SecurityAlgorithms.HmacSha256Signature),
            Expires = DateTime.Now.AddMinutes(AccessTokenValidityInMinutes),
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = tokenHandler.CreateToken(tokenDescriptor);

        return new AccessTokenResponse()
        {
            Value = tokenHandler.WriteToken(jwtSecurityToken),
            ExpiresAt = jwtSecurityToken.ValidTo
        };
    }

    private static string GenerateRefreshToken(List<string> usedRefreshTokens)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var tokenIsUnique = !usedRefreshTokens.Any(usedToken => usedToken == token);
        if (!tokenIsUnique)
            return GenerateRefreshToken(usedRefreshTokens);

        return token;
    }
    public static RefreshToken CreateRefreshToken(string userId, List<string> usedRefreshTokens, string ipAddress = null)
    {
        return new RefreshToken
        {
            Value = GenerateRefreshToken(usedRefreshTokens),
            CreatedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddDays(RefreshTokenValidityInDays),
            UserId = userId
        };
    }

    public static TokensResponse GetTokens(AccessTokenResponse accessToken, RefreshToken refreshToken)
    {
        return new TokensResponse()
        {
            AccessToken = accessToken,
            RefreshToken = new RefreshTokenResponse() { Value = refreshToken.Value, ExpiresAt = refreshToken.ExpiresAt }
        };
    }

}
