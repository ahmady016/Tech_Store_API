using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Auth.Commands;

public class ConfirmEmailCommand : IRequest<IResult>
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
}

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IResultService _resultService;
    public ConfirmEmailCommandHandler(
        UserManager<User> userManager,
        IResultService resultService
    )
    {
        _userManager = userManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken
    )
    {
        // get currentUser by Id
        var currentUser = await _userManager.FindByIdAsync(command.UserId.ToString());
        // if currentUser not found
        if (currentUser is null)
            return _resultService.NotFound(nameof(ConfirmEmailCommand), nameof(User), command.UserId);

        // decode the Token
        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.Token));
        // confirmEmail
        var identityResult = await _userManager.ConfirmEmailAsync(currentUser, decodedToken);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(ConfirmEmailCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );
        // return result message
        return _resultService.Succeeded("Email is successfully confirmed, you can signin now");
    }
}
