using Microsoft.AspNetCore.Identity;
using MediatR;
using Mapster;

using TechStoreApi.Common;
using TechStoreApi.Entities;
using TechStoreApi.Dtos;

namespace TechStoreApi.Admin.Queries;

public class FindRoleQuery : IdInput {}

public class FindRoleQueryHandler : IRequestHandler<FindRoleQuery, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IResultService _resultService;
    public FindRoleQueryHandler(
        RoleManager<Role> roleManager,
        UserManager<User> userManager,
        IResultService resultService
    )
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        FindRoleQuery query,
        CancellationToken cancellationToken
    )
    {
        var existingRole = await _roleManager.FindByIdAsync(query.Id.ToString());
        if(existingRole is null)
            return _resultService.NotFound(nameof(FindRoleQuery), nameof(Role), query.Id);

        var roleUsers = await _userManager.GetUsersInRoleAsync(existingRole.Name);

        var roleDto = existingRole.Adapt<RoleDto>();
        roleDto.Users = roleUsers.Adapt<List<UserDto>>();
        return Results.Ok(roleDto);
    }
}
