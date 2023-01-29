
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.DB;
using TechStoreApi.Entities;

namespace TechStoreApi.Auth.Commands;

public class SignoutCommand : IRequest<IResult> {}

public class SignoutCommandHandler : IRequestHandler<SignoutCommand, IResult>
{
    private readonly IHttpContextAccessor _httpAccessor;
    private readonly UserManager<User> _userManager;
    private readonly IAuthService _authService;
    private readonly IRawDBService _rawDbService;
    private readonly IResultService _resultService;
    public SignoutCommandHandler(
        IHttpContextAccessor httpAccessor,
        UserManager<User> userManager,
        IAuthService authService,
        IRawDBService rawDbService,
        IResultService resultService
    )
    {
        _httpAccessor = httpAccessor;
        _userManager = userManager;
        _authService = authService;
        _rawDbService = rawDbService;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        SignoutCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = JwtService.GetCurrentUserId(_httpAccessor.HttpContext);
        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var currentUser = await _userManager.FindByIdAsync(userId);
        if (currentUser is null)
            return Results.Unauthorized();

        var deleteTokensResult = await _rawDbService.ExecuteDeleteAsync<RefreshToken>(e => e.UserId == Guid.Parse(userId));
        if(deleteTokensResult == 0)
            return _resultService.Conflict(nameof(SignoutCommand), "User already logged Out !!!");

        return _resultService.Succeeded("User has been logged Out successfully ...");
    }
}
