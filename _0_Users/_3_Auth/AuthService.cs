using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Net;
using AutoMapper;

using DB;
using Entities;

namespace Auth;

public interface IAuthService
{
    RefreshToken GetRefreshToken(string token);
    void RevokeToken(RefreshToken token, string reason = null);
    Task RevokeAllUserRefreshTokensAsync(string userId);
    Task RemoveAllExpiredOrRevokedTokensAsync(string userId);
    Task<TokensResponse> GenerateTokensAsync(User existedUser);

    Guid? GetCurrentUserId();
    string GetCurrentUserEmail();
    string GetCurrentUserName();
    Task<User> GetCurrentUser();
}

public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpAccessor;
    private readonly IDBService _dbService;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public AuthService(
        IHttpContextAccessor httpAccessor,
        IDBService dbService,
        UserManager<User> userManager,
        IMapper mapper,
        ILogger<User> logger
    )
    {
        _httpAccessor = httpAccessor;
        _dbService = dbService;
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    public void RevokeToken(RefreshToken token, string reason = null)
    {
        token.RevokedAt = DateTime.Now;
        token.RevokedReason = reason;
    }

    public RefreshToken GetRefreshToken(string token)
    {
        var refreshToken = _dbService.GetQuery<RefreshToken>()
            .Include(rf => rf.User)
            .Where(rf => rf.Value == token)
            .FirstOrDefault();

        if (refreshToken is null)
        {
            _errorMessage = "Token Not Found !!!";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Unauthorized);
        }

        return refreshToken;
    }
    public async Task RevokeAllUserRefreshTokensAsync(string userId)
    {
        var allUserTokens = await _dbService.GetListAsync<RefreshToken>(token => token.UserId == userId);
        if (allUserTokens.Count > 0)
        {
            allUserTokens.ForEach(token => RevokeToken(token, $"Attempted to reuse a revoked token"));
            _dbService.UpdateRange<RefreshToken>(allUserTokens);
            await _dbService.SaveChangesAsync();
        }
    }
    public async Task RemoveAllExpiredOrRevokedTokensAsync(string userId)
    {
        _dbService.GetListAndDelete<RefreshToken>(token =>
            token.UserId == userId && (token.RevokedAt != null || token.ExpiresAt < DateTime.Now)
        );
        await _dbService.SaveChangesAsync();
    }
    public async Task<TokensResponse> GenerateTokensAsync(User existedUser)
    {
        // add claims and generate accessToken
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, existedUser.Id),
            new Claim(ClaimTypes.Name, existedUser.UserName),
            new Claim(ClaimTypes.Email, existedUser.Email),
        };
        var userRoles = await _userManager.GetRolesAsync(existedUser);
        foreach (var userRole in userRoles)
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        var accessToken = AuthHelpers.GenerateAccessToken(claims);

        // generate and save refreshToken
        var usedRefreshTokens = _dbService.GetQuery<RefreshToken>()
            .Select(r => r.Value)
            .ToList();
        var refreshToken = AuthHelpers.CreateRefreshToken(existedUser.Id, usedRefreshTokens);
        _dbService.Add<RefreshToken>(refreshToken);
        await _dbService.SaveChangesAsync();

        return AuthHelpers.GetTokens(accessToken, refreshToken);
    }

    private List<Claim> GetClaims()
    {
        var token = _httpAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (String.IsNullOrEmpty(token) is not false)
        {
            var tokenValidationParameters = AuthHelpers.GetTokenValidationOptions(validateLifetime: true);
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            return principal.Claims.ToList();
        }
        return null;
    }
    public Guid? GetCurrentUserId()
    {
        var claims = GetClaims();
        if (claims is not null)
        {
            var userId = claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value;
            if(String.IsNullOrEmpty(userId) is not false)
                return Guid.Parse(userId);
        }
        return null;
    }
    public string GetCurrentUserEmail()
    {
        var claims = GetClaims();
        if (claims is not null)
        {
            var userEmail = claims.FirstOrDefault(e => e.Type == ClaimTypes.Email).Value;
            if(String.IsNullOrEmpty(userEmail) is not false)
                return userEmail;
        }
        return null;
    }
    public string GetCurrentUserName()
    {
        var claims = GetClaims();
        if (claims is not null)
        {
            var userName = claims.FirstOrDefault(e => e.Type == ClaimTypes.Name).Value;
            if(String.IsNullOrEmpty(userName) is not false)
                return userName;
        }
        return null;
    }
    public async Task<User> GetCurrentUser()
    {
        var claims = GetClaims();
        if (claims is not null)
        {
            var userEmail = claims.FirstOrDefault(e => e.Type == ClaimTypes.Email).Value;
            if(String.IsNullOrEmpty(userEmail) is not false)
                return await _userManager.FindByEmailAsync(userEmail);
        }
        return null;
    }
}
