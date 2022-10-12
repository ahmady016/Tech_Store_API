using System.ComponentModel.DataAnnotations;
using MediatR;

using DB;
using Entities;

namespace Auth.Commands;

public class RefreshTokensCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Token is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "Token Must be between 10 and 450 characters")]
    public string Token { get; set; }
}

public class RefreshTokensCommandHandler : IRequestHandler<RefreshTokensCommand, IResult> {
    private readonly IAuthService _authService;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public RefreshTokensCommandHandler(
        IAuthService authService,
        ILogger<User> logger
    )
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<IResult> Handle (
        RefreshTokensCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedRefreshToken = _authService.GetRefreshToken(command.Token);
        if (!existedRefreshToken.IsValid)
        {
            _errorMessage = "Invalid Token !!!";
            _logger.LogError(_errorMessage);
            return Results.Unauthorized();
        }

        // revoke all tokens of that user in case of this token has been compromised
        if (existedRefreshToken.IsRevoked)
            await _authService.RevokeAllUserRefreshTokensAsync(existedRefreshToken.UserId);

        await _authService.RemoveAllExpiredOrRevokedTokensAsync(existedRefreshToken.UserId);

        var tokens = await _authService.GenerateTokensAsync(existedRefreshToken.User);
        return Results.Ok(new AuthDto() { User = existedRefreshToken.User, Tokens = tokens });
    }

}
