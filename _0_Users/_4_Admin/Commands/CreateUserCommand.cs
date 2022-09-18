using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MediatR;

using Entities;
using Common;
using Dtos;

namespace Admin.Commands;

public class CreateUserCommand : UserInput {}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResult>
{
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _config;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
        public CreateUserCommandHandler(
        IMapper mapper,
        IEmailService emailService,
        UserManager<User> userManager,
        IConfiguration Configuration,
        ILogger<Product> logger
    )
    {
        _mapper = mapper;
        _emailService = emailService;
        _userManager = userManager;
        _config = Configuration;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        var newUser = _mapper.Map<User>(command);
        var identityResult = await _userManager.CreateAsync(newUser, command.Password);
        if(identityResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }

        // generate email confirmation token and send confirmation email
        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        var confirmationUrl = new Uri($"{_config["BaseUrl"]}/api/Auth/ConfirmEmail?userId={newUser.Id}&token={confirmationToken}");
        await _emailService.SendAsync(
            newUser.Email,
            "Please Confirm Your Email",
            $"Please Click on this link to confirm your email: {confirmationUrl}"
        );
        return Results.Ok(new { Message = "User created successfully and confirmation email was sent" });
    }
}
