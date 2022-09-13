using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Auth.Commands;
namespace Auth;

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
    private static int TokenValidityInMinutes => int.Parse(_config["JWT:TokenValidityInMinutes"]);

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

    public static TokenResponse GenerateAccessToken(List<Claim> claims)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = TokenIssuer,
            Audience = TokenAudience,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(TokenSecret), SecurityAlgorithms.HmacSha256Signature),
            Expires = DateTime.Now.AddMinutes(TokenValidityInMinutes),
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = tokenHandler.CreateToken(tokenDescriptor);
        return new TokenResponse()
        {
            AccessToken = tokenHandler.WriteToken(jwtSecurityToken),
            ExpiresAt = jwtSecurityToken.ValidTo
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

}
