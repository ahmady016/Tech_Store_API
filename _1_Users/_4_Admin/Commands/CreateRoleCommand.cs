using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Mapster;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class CreateRoleCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(900, MinimumLength = 5, ErrorMessage = "Name Must between 5 and 900 characters")]
    public string Name { get; set; }
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IResultService _resultService;
    public CreateRoleCommandHandler(
        RoleManager<Role> roleManager,
        IResultService resultService
    )
    {
        _roleManager = roleManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        CreateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var newRole = command.Adapt<Role>();
        var identityResult = await _roleManager.CreateAsync(newRole);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(CreateRoleCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );

        return _resultService.Succeeded("Role created successfully ...");
    }
}
