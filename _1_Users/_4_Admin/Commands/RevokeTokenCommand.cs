using System.ComponentModel.DataAnnotations;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.DB;
using TechStoreApi.Auth;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class RevokeTokenCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Token is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "Token Must be between 10 and 450 characters")]
    public string Token { get; set; }
    public string RevokedReason { get; set; }
}

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IRawDBService _rawDBService;
    private readonly IResultService _resultService;
    public RevokeTokenCommandHandler(
        IAuthService authService,
        IRawDBService rawDBService,
        IResultService resultService
    )
    {
        _authService = authService;
        _rawDBService = rawDBService;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        RevokeTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existingToken from db
        var existingToken =  await _authService.GetRefreshToken(command.Token);
        // if existingToken is not valid
        if (existingToken.IsValid is false)
            return _resultService.Conflict(nameof(RevokeTokenCommand), "Refresh Token is Expired Or Revoked already");

        // if revokedReason given
        var revokedReason = command.RevokedReason.Trim();
        var IsRevokedReasonEmpty = string.IsNullOrEmpty(revokedReason);
        if(!IsRevokedReasonEmpty && !revokedReason.IsLengthBetween(3, 450))
            return _resultService.BadRequest(nameof(RevokeTokenCommand), "Revoked Reason must be between 3 and 450 characters!!!");

        // Revoke RefreshToken
        _authService.RevokeRefreshToken(existingToken, IsRevokedReasonEmpty ? null : revokedReason);
        _rawDBService.Update<RefreshToken>(existingToken);
        await _rawDBService.SaveChangesAsync();

        return _resultService.Succeeded("Refresh Token has been revoked successfully ...");
    }
}
