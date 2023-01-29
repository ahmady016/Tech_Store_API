using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Mapster;

using TechStoreApi.Common;
using TechStoreApi.DB;
using TechStoreApi.Entities;
using TechStoreApi.Dtos;

namespace TechStoreApi.Admin.Queries;

public class FindUserQuery : IdInput {}

public class FindUserQueryHandler : IRequestHandler<FindUserQuery, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IRawDBService _rawDBService;
    private readonly IResultService _resultService;
    public FindUserQueryHandler(
        UserManager<User> userManager,
        IRawDBService rawDBService,
        IResultService resultService
    )
    {
        _userManager = userManager;
        _rawDBService = rawDBService;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        FindUserQuery query,
        CancellationToken cancellationToken
    )
    {
        var existingUser = await _userManager.FindByIdAsync(query.Id.ToString());
        if (existingUser is null)
            return _resultService.NotFound(nameof(FindUserQuery), nameof(User), query.Id);

        existingUser.Roles = await _rawDBService.GetQuery<UserRole>()
            .Include(e => e.Role)
            .Where(e => e.UserId == existingUser.Id)
            .ToListAsync();
        var userDto = existingUser.Adapt<UserDto>();
        var roles = existingUser.Roles.Select(e => e.Role);
        userDto.Roles = roles.Adapt<List<RoleDto>>();
        return Results.Ok(userDto);
    }
}
