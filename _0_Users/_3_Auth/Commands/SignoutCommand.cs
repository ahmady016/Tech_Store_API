using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using DB;
using Entities;

namespace Auth.Commands;

public class SignoutCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "UserId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "UserId Must be between 10 and 450 characters")]
    public string UserId { get; set; }
}

public class SignoutCommandHandler : IRequestHandler<SignoutCommand, IResult> {
    private readonly UserManager<User> _userManager;
    private readonly IDBService _dbService;
    private readonly IAuthService _authService;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public SignoutCommandHandler(
        UserManager<User> userManager,
        IDBService dbService,
        IAuthService authService,
        ILogger<User> logger
    )
    {
        _userManager = userManager;
        _dbService = dbService;
        _authService = authService;
        _logger = logger;
    }

    public async Task<IResult> Handle (
        SignoutCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedUser = await _userManager.FindByIdAsync(command.UserId);
        if (existedUser is null)
        {
            _errorMessage = $"User with Id: {command.UserId} not Found !!!";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(_errorMessage);
        }

        bool foundTokens = _dbService.GetListAndDelete<RefreshToken>(token => token.UserId == command.UserId);
        if (foundTokens is false)
        {
            _errorMessage = "User already loggedOut !!!";
            _logger.LogError(_errorMessage);
            return Results.Conflict(_errorMessage);
        }

        await _dbService.SaveChangesAsync();
        return Results.Ok(new { Message = "User is loggedOut successfully ..." });
    }

}
