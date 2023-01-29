using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class UpdateRoleCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is Required")]
    [StringLength(900, MinimumLength = 5, ErrorMessage = "Name must between 5 and 900 characters")]
    public string Name { get; set; }
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IResultService _resultService;
    public UpdateRoleCommandHandler(
        RoleManager<Role> roleManager,
        IResultService resultService
    )
    {
        _roleManager = roleManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        UpdateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existingRole from db
        var existingRole = await _roleManager.FindByIdAsync(command.Id.ToString());
        // if existingRole not found
        if(existingRole is null)
            return _resultService.NotFound(nameof(UpdateRoleCommand), nameof(Role), command.Id);

        // if no field changed
        var isNameChanged = command.Name != existingRole.Name;
        if(!isNameChanged)
            return _resultService.BadRequest(
                nameof(UpdateRoleCommand),
                "nothing to update, role name not changed!!!"
            );

        // update existingRole
        existingRole.Name = command.Name;
        var identityResult = await _roleManager.UpdateAsync(existingRole);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(UpdateRoleCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );

        return _resultService.Succeeded("Role updated successfully ...");
    }
}
