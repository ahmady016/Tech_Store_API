using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace TechStoreApi.Auth;

public class TokenDto
{
    public string Value { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public static class JwtService
{
    private static IConfiguration _config;
    public static void Initialize(IConfiguration Configuration) => _config = Configuration;

    public static string TokenIssuer => _config["JWT:Issuer"];
    public static string TokenAudience => _config["JWT:Audience"];
    public static byte[] TokenSecret => Encoding.UTF8.GetBytes(_config["JWT:Secret"]);
    public static int AccessTokenValidityInMinutes => int.Parse(_config["JWT:AccessTokenValidityInMinutes"]);
    public static int RefreshTokenValidityInDays => int.Parse(_config["JWT:RefreshTokenValidityInDays"]);

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
    public static TokenDto GenerateAccessToken(List<Claim> claims)
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

        return new TokenDto()
        {
            Value = tokenHandler.WriteToken(jwtSecurityToken),
            ExpiresAt = jwtSecurityToken.ValidTo
        };
    }
    public static string GenerateRefreshToken(List<string> usedRefreshTokens)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        if (usedRefreshTokens.Contains(token))
            return GenerateRefreshToken(usedRefreshTokens);
        return token;
    }
    public static bool ValidateToken(string token)
    {
        var tokenValidationParameters = GetTokenValidationOptions(validateLifetime: true);
        var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        return (
            securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
        )
            ? false
            : true;
    }
    public static List<Claim> GetClaims(HttpContext httpContext)
    {
        var authHeader = httpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring(authHeader.IndexOf(' ')+1);
        if (string.IsNullOrEmpty(token) is false)
        {
            var tokenValidationParameters = GetTokenValidationOptions(validateLifetime: true);
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            return principal.Claims.ToList();
        }
        return null;
    }
    public static string GetCurrentUserId(HttpContext httpContext)
    {
        var claims = GetClaims(httpContext);
        return (claims is not null)
            ? claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value
            : null;
    }
    public static string GetCurrentUserEmail(HttpContext httpContext)
    {
        var claims = GetClaims(httpContext);
        if (claims is not null)
        {
            var userEmail = claims.FirstOrDefault(e => e.Type == ClaimTypes.Email).Value;
            if (String.IsNullOrEmpty(userEmail) is false)
                return userEmail;
        }
        return null;
    }
    public static string GetCurrentUserName(HttpContext httpContext)
    {
        var claims = GetClaims(httpContext);
        if (claims is not null)
        {
            var userName = claims.FirstOrDefault(e => e.Type == ClaimTypes.Name).Value;
            if (String.IsNullOrEmpty(userName) is false)
                return userName;
        }
        return null;
    }
}
