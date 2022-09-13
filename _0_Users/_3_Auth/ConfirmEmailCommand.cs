using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using Entities;

namespace Auth.Commands;

public class ConfirmEmailCommand : IRequest<IResult>
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string Token { get; set; }
}

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, IResult> {
    private readonly UserManager<User> _userManager;
    public ConfirmEmailCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IResult> Handle (
        ConfirmEmailCommand request,
        CancellationToken cancellationToken
    )
    {
        var existedUser = await _userManager.FindByIdAsync(request.UserId);
        if(existedUser is not null)
        {
            var identityResult = await _userManager.ConfirmEmailAsync(existedUser, request.Token);
            if(identityResult.Succeeded)
                return Results.Ok(new { Message = "Email is successfully confirmed, you can signin now" });
        }

        return Results.Unauthorized();
    }

}
