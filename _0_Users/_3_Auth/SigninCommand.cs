using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
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
    private readonly IAuthService _authService;
    public SigninCommandHandler(
        IConfiguration Configuration,
        UserManager<User> userManager,
        IDBService dbService,
        IAuthService authService
    )
    {
        _config = Configuration;
        _userManager = userManager;
        _dbService = dbService;
        _authService = authService;
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
                var tokens = await _authService.GenerateTokensAsync(existedUser);
                return Results.Ok(new AuthDto() { User = existedUser, Tokens = tokens });
            }
        }

        return Results.Unauthorized();
    }

}
