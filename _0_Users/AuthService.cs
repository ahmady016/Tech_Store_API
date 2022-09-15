using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
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
    void RevokeAllUserRefreshTokens(string userId);
    void RemoveAllExpiredOrRevokedTokens(string userId);
    Task<TokensResponse> GenerateTokensAsync(User existedUser);
}

public class AuthService : IAuthService
{
    private readonly IDBService _dbService;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public AuthService(
        IDBService dbService,
        UserManager<User> userManager,
        IMapper mapper,
        ILogger<User> logger
    )
    {
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
    public void RevokeAllUserRefreshTokens(string userId)
    {
        var allUserTokens = _dbService.GetList<RefreshToken>(token => token.UserId == userId);
        if (allUserTokens.Count > 0)
        {
            allUserTokens.ForEach(token => RevokeToken(token, $"Attempted to reuse a revoked token"));
            _dbService.UpdateRange<RefreshToken>(allUserTokens);
            _dbService.SaveChanges();
        }
    }
    public void RemoveAllExpiredOrRevokedTokens(string userId)
    {
        _dbService.GetListAndDelete<RefreshToken>(token =>
            token.UserId == userId && (token.RevokedAt != null || token.ExpiresAt < DateTime.Now)
        );
        _dbService.SaveChanges();
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
        _dbService.SaveChanges();

        return AuthHelpers.GetTokens(accessToken, refreshToken);
    }

}
