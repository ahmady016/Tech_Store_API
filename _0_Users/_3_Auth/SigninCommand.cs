using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MediatR;

using Entities;

namespace Auth.Commands;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}

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
    public SigninCommandHandler(
        IConfiguration Configuration,
        UserManager<User> userManager
    )
    {
        _userManager = userManager;
        _config = Configuration;
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
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, existedUser.Id),
                    new Claim(ClaimTypes.Name, existedUser.UserName),
                    new Claim(ClaimTypes.Email, existedUser.Email),
                };
                var userRoles = await _userManager.GetRolesAsync(existedUser);
                foreach (var userRole in userRoles)
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                return Results.Ok(AuthHelpers.GenerateAccessToken(claims));
            }
        }

        return Results.Unauthorized();
    }

}
