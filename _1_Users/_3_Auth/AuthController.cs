using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;

using TechStoreApi.Auth.Commands;

namespace TechStoreApi.Clubs;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public Task<IResult> Signup(SignupCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpGet]
    public Task<IResult> ConfirmEmail([FromQuery][AsParameters] ConfirmEmailCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpPost]
    public Task<IResult> Signin(SigninCommand command)
    {
        return _mediator.Send(command);
    }
    [Authorize]
    [HttpPost]
    public Task<IResult> Signout(SignoutCommand command)
    {
        return _mediator.Send(command);
    }

    [HttpPost]
    public Task<IResult> RefreshTokens(RefreshTokensCommand command)
    {
        return _mediator.Send(command);
    }

    [HttpPost]
    public Task<IResult> ForgotPassword(ForgotPasswordCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpPost]
    public Task<IResult> ResetPassword(ResetPasswordCommand command)
    {
        return _mediator.Send(command);
    }
    [Authorize]
    [HttpPost]
    public Task<IResult> ChangeEmail(ChangeEmailCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpGet]
    public Task<IResult> ConfirmChangeEmail([FromQuery][AsParameters] ConfirmChangeEmailCommand command)
    {
        return _mediator.Send(command);
    }
    [Authorize]
    [HttpPost]
    public Task<IResult> ChangePassword(ChangePasswordCommand command)
    {
        return _mediator.Send(command);
    }
}
