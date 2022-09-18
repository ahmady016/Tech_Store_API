using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MediatR;

using Entities;

namespace Admin.Commands;

public class UpdateRoleCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Id is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "Id Must between 10 and 450 characters")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Name is Required")]
    [StringLength(900, MinimumLength = 5, ErrorMessage = "Name must between 5 and 900 characters")]
    public string Name { get; set; }
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResult>
{
    private readonly IMapper _mapper;
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
        public UpdateRoleCommandHandler(
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
        UpdateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedRole = await _roleManager.FindByIdAsync(command.Id);
        existedRole.Name = command.Name;
        var identityResult = await _roleManager.UpdateAsync(existedRole);
        if(identityResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }

        return Results.Ok(new { Message = "Role Updated successfully ..." });
    }
}
