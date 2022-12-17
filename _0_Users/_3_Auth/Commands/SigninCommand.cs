using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using Entities;

namespace Auth.Commands;

public class SigninCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Email Must be between 10 and 100 characters")]
    [EmailAddress(ErrorMessage = "Email Must be a valid email")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required")]
    [StringLength(64, MinimumLength = 8, ErrorMessage = "Password Must be between 8 and 64 characters")]
    public string Password { get; set; }
}

public class SigninCommandHandler : IRequestHandler<SigninCommand, IResult> {
    private readonly UserManager<User> _userManager;
    private readonly IAuthService _authService;
    public SigninCommandHandler(
        UserManager<User> userManager,
        IAuthService authService
    )
    {
        _userManager = userManager;
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
            if(isValidPassword && existedUser.EmailConfirmed)
            {
                var tokens = await _authService.GenerateTokensAsync(existedUser);
                return Results.Ok(new AuthDto() { User = existedUser, Tokens = tokens });
            }
        }

        return Results.Unauthorized();
    }

}
