using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Auth.Commands;

public class ConfirmChangeEmailCommand : IRequest<IResult>
{
    public Guid UserId { get; set; }
    public string NewEmail { get; set; }
    public string Token { get; set; }
}

public class ConfirmChangeEmailCommandHandler : IRequestHandler<ConfirmChangeEmailCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IResultService _resultService;
    public ConfirmChangeEmailCommandHandler(
        UserManager<User> userManager,
        IResultService resultService
    )
    {
        _userManager = userManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        ConfirmChangeEmailCommand command,
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
        // change and confirm newEmail
        currentUser.UserName = command.NewEmail;
        var identityResult = await _userManager.ChangeEmailAsync(currentUser, command.NewEmail, decodedToken);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(ConfirmEmailCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );
        // return result message
        return _resultService.Succeeded("New Email is successfully changed and confirmed, you can signin now");
    }
}
