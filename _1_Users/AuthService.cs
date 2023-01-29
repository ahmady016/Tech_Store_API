using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mapster;

using TechStoreApi.DB;
using TechStoreApi.Entities;
using TechStoreApi.Dtos;

namespace TechStoreApi.Auth;

public interface IAuthService
{
    Task<RefreshToken> GetRefreshToken(string token);
    void RevokeRefreshToken(RefreshToken token, string reason = null);
    Task RemoveAllExpiredOrRevokedTokensAsync(Guid userId);
    Task RevokeAllUserRefreshTokensAsync(Guid userId);
    Task<TokensDto> GenerateTokensAsync(User user);
    Task<User> GetCurrentUser();
}

public class AuthService : IAuthService
{
    private readonly TechStoreDB _db;
    private readonly IHttpContextAccessor _httpAccessor;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public AuthService(
        TechStoreDB db,
        IHttpContextAccessor httpAccessor,
        UserManager<User> userManager,
        ILogger<User> logger
    )
    {
        _db = db;
        _httpAccessor = httpAccessor;
        _userManager = userManager;
        _logger = logger;
    }

    private RefreshToken CreateRefreshToken(Guid userId, List<string> usedRefreshTokens)
    {
        return new RefreshToken
        {
            UserId = userId,
            Value = JwtService.GenerateRefreshToken(usedRefreshTokens),
            ExpiresAt = DateTime.Now.AddDays(JwtService.RefreshTokenValidityInDays)
        };
    }
    public void RevokeRefreshToken(RefreshToken token, string reason = null)
    {
        token.RevokedAt = DateTime.Now;
        token.RevokedReason = reason;
    }
    public async Task<RefreshToken> GetRefreshToken(string token)
    {
        var refreshToken = await _db.RefreshTokens
            .Include(rf => rf.User)
            .Where(rf => rf.Value == token)
            .FirstOrDefaultAsync();
        if (refreshToken is null)
        {
            _errorMessage = $"{nameof(AuthService)} => Token Not Found !!!";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Unauthorized);
        }
        return refreshToken;
    }
    public async Task RevokeAllUserRefreshTokensAsync(Guid userId)
    {
        await _db.RefreshTokens
            .Where(token => token.UserId == userId)
            .ExecuteUpdateAsync(e => e
                .SetProperty(e => e.RevokedAt, e => DateTime.Now)
                .SetProperty(e => e.RevokedReason, e => "Attempted to reuse a revoked token")
            );
    }
    public async Task RemoveAllExpiredOrRevokedTokensAsync(Guid userId)
    {
        await _db.RefreshTokens
            .Where(token => token.UserId == userId && (token.RevokedAt != null || token.ExpiresAt <= DateTime.Now))
            .ExecuteDeleteAsync();
    }
    public async Task<TokensDto> GenerateTokensAsync(User user)
    {
        // add basic user claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
        };
        // add user role(s) claims
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var userRole in userRoles)
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        // generate accessToken using claims
        var accessToken = JwtService.GenerateAccessToken(claims);

        // get all used RefreshTokens
        var allRefreshTokens = await _db.RefreshTokens
            .OrderBy(r => r.ExpiresAt)
            .Select(r => new { Value = r.Value, UserId = r.UserId })
            .ToListAsync();
        // keep only the newest four of each user
        var userRefreshTokens = allRefreshTokens
            .Where(r => r.UserId == user.Id)
            .Select(r => r.Value)
            .ToList();
        if(userRefreshTokens.Count >= 5)
            await _db.RefreshTokens
                .Where(r => r.Value == userRefreshTokens.First())
                .ExecuteDeleteAsync();
        // generate and save refreshToken
        var allUsedTokens = allRefreshTokens.Select(r => r.Value).ToList();
        var refreshToken = CreateRefreshToken(user.Id, allUsedTokens);
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();
        // return TokensDto
        return new TokensDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Adapt<TokenDto>()
        };
    }
    public async Task<User> GetCurrentUser()
    {
        var claims = JwtService.GetClaims(_httpAccessor.HttpContext);
        if (claims is not null)
        {
            var userEmail = claims.FirstOrDefault(e => e.Type == ClaimTypes.Email).Value;
            if (String.IsNullOrEmpty(userEmail) is not false)
                return await _userManager.FindByEmailAsync(userEmail);
        }
        return null;
    }
}
