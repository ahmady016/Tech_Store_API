using MediatR;
using Microsoft.AspNetCore.Mvc;

using Auth.Commands;

namespace Auth;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Auth/Signup
    /// </summary>
    /// <returns>Message</returns>
    [HttpPost]
    public Task<IResult> Signup(SignupCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Auth/ConfirmEmail
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> ConfirmEmail(ConfirmEmailCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Auth/Signin
    /// </summary>
    /// <returns>AuthDto</returns>
    [HttpPost]
    public Task<IResult> Signin(SigninCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Auth/RefreshTokens
    /// </summary>
    /// <returns>AuthDto</returns>
    [HttpPost]
    public Task<IResult> RefreshTokens(RefreshTokensCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Auth/Signout
    /// </summary>
    /// <returns>Message</returns>
    [HttpPost]
    public Task<IResult> Signout(SignoutCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Auth/SendConfirmationEmail
    /// </summary>
    /// <returns>Message</returns>
    [HttpPost]
    public Task<IResult> SendConfirmationEmail(SendConfirmationEmailCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Auth/ChangeEmail
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> ChangeEmail(ChangeEmailCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Auth/ChangePassword
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> ChangePassword(ChangePasswordCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Auth/ForgotPassword
    /// </summary>
    /// <returns>Message</returns>
    [HttpPost]
    public Task<IResult> ForgotPassword(ForgotPasswordCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Auth/ResetPassword
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> ResetPassword(ResetPasswordCommand command)
    {
        return _mediator.Send(command);
    }

}
