using System.ComponentModel.DataAnnotations;
using MediatR;

using DB;
using Entities;
using Auth;

namespace Admin.Commands;

public class RevokeTokenCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Token is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "Token Must be between 10 and 450 characters")]
    public string Token { get; set; }
}

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, IResult> {
    private readonly IDBService _dbService;
    private readonly IAuthService _authService;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public RevokeTokenCommandHandler(
        IDBService dbService,
        IAuthService authService,
        ILogger<User> logger
    )
    {
        _dbService = dbService;
        _authService = authService;
        _logger = logger;
    }

    public async Task<IResult> Handle (
        RevokeTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedRefreshToken = _authService.GetRefreshToken(command.Token);
        if (!existedRefreshToken.IsValid)
        {
            _errorMessage = "Invalid Token !!!";
            _logger.LogError(_errorMessage);
            return await Task.FromResult(
                Results.BadRequest(new { Message = "Refresh Token is Expired Or Revoked already" })
            );
        }

        _authService.RevokeToken(existedRefreshToken);
        _dbService.Update<RefreshToken>(existedRefreshToken);
        _dbService.SaveChanges();
        return await Task.FromResult(
            Results.Ok(new { Message = "Refresh Token was revoked successfully" })
        );
    }

}
