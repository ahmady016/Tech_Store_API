using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MediatR;

using Entities;

namespace Admin.Commands;

public class CreateRoleCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(900, MinimumLength = 5, ErrorMessage = "Name Must between 5 and 900 characters")]
    public string Name { get; set; }
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IResult>
{
    private readonly IMapper _mapper;
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
        public CreateRoleCommandHandler(
        IMapper mapper,
        RoleManager<Role> roleManager,
        ILogger<Product> logger
    )
    {
        _mapper = mapper;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        CreateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var newRole = _mapper.Map<Role>(command);
        var identityResult = await _roleManager.CreateAsync(newRole);
        if(identityResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }

        return Results.Ok(new { Message = "Role created successfully ..." });
    }
}
