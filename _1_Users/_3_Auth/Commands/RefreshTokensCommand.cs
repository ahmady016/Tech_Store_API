
using System.ComponentModel.DataAnnotations;
using MediatR;
using Mapster;

using TechStoreApi.Common;
using TechStoreApi.Dtos;

namespace TechStoreApi.Auth.Commands;

public class RefreshTokensCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Token is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "Token Must be between 10 and 450 characters")]
    public string Token { get; set; }
}

public class RefreshTokensCommandHandler : IRequestHandler<RefreshTokensCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IResultService _resultService;
    public RefreshTokensCommandHandler(
        IAuthService authService,
        IResultService resultService
    )
    {
        _authService = authService;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        RefreshTokensCommand command,
        CancellationToken cancellationToken
    )
    {
        // if refreshToken was expired and/or revoked return Unauthorized
        var existingRefreshToken = await _authService.GetRefreshToken(command.Token);
        if (existingRefreshToken.IsValid is false)
        {
            if (existingRefreshToken.IsRevoked) // revoke all tokens of that user in case of this token has been compromised
                await _authService.RevokeAllUserRefreshTokensAsync(existingRefreshToken.UserId);
            return Results.Unauthorized();
        }

        // refresh and return new tokens
        await _authService.RemoveAllExpiredOrRevokedTokensAsync(existingRefreshToken.UserId);
        var tokensDto = await _authService.GenerateTokensAsync(existingRefreshToken.User);
        var userDto = existingRefreshToken.User.Adapt<UserDto>();
        return Results.Ok(new AuthDto() { User = userDto, Tokens = tokensDto });
    }
}
