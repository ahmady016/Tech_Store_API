using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MediatR;

using DB;
using Entities;

namespace Auth.Commands;

public class SigninCommand : IRequest<IResult>
{
    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(50)]
    public string Password { get; set; }
}

public class SigninCommandHandler : IRequestHandler<SigninCommand, IResult> {
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    private readonly IDBService _dbService;
    public SigninCommandHandler(
        IConfiguration Configuration,
        UserManager<User> userManager,
        IDBService dbService
    )
    {
        _config = Configuration;
        _userManager = userManager;
        _dbService = dbService;
    }

    public async Task<IResult> Handle (
        SigninCommand request,
        CancellationToken cancellationToken
    )
    {
        var existedUser = await _userManager.FindByEmailAsync(request.Email);
        if(existedUser is not null)
        {
            var isValidPassword = await _userManager.CheckPasswordAsync(existedUser, request.Password);
            if(isValidPassword)
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

                return Results.Ok(AuthHelpers.GetTokens(accessToken, refreshToken));
            }
        }

        return Results.Unauthorized();
    }

}
