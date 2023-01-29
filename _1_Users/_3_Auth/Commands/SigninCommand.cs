
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Mapster;

using TechStoreApi.Common;
using TechStoreApi.Entities;
using TechStoreApi.Dtos;

namespace TechStoreApi.Auth.Commands;

public class SigninCommand : IRequest<IResult>
{
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Email Must be between 10 and 100 characters")]
    [EmailAddress(ErrorMessage = "Email Must be a valid email")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required")]
    [StringLength(64, MinimumLength = 8, ErrorMessage = "Password Must be between 8 and 64 characters")]
    public string Password { get; set; }
}

public class SigninCommandHandler : IRequestHandler<SigninCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IAuthService _authService;
    private readonly IResultService _resultService;
    public SigninCommandHandler(
        UserManager<User> userManager,
        IAuthService authService,
        IResultService resultService
    )
    {
        _userManager = userManager;
        _authService = authService;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        SigninCommand command,
        CancellationToken cancellationToken
    )
    {
        var existingUser = await _userManager.FindByEmailAsync(command.Email);
        if(existingUser is null)
            return _resultService.BadRequest(nameof(SigninCommand), "invalid user credentials");

        if(existingUser.EmailConfirmed is false)
            return _resultService.BadRequest(
                nameof(SigninCommand),
                $"This email: {command.Email} is not confirmed, please confirm the email first"
            );

        var isValidPassword = await _userManager.CheckPasswordAsync(existingUser, command.Password);
        if(isValidPassword is false)
            return _resultService.BadRequest(nameof(SigninCommand), "invalid user credentials");

        var tokensDto = await _authService.GenerateTokensAsync(existingUser);
        var userDto = existingUser.Adapt<UserDto>();
        return Results.Ok(new AuthDto() { User = userDto, Tokens = tokensDto });
    }
}
