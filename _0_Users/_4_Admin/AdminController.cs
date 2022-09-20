using MediatR;
using Microsoft.AspNetCore.Mvc;

using Admin.Queries;
using Admin.Commands;

namespace Admin;

[ApiController]
[Route("api/[controller]/[action]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Admin/ListRoles
    /// </summary>
    /// <returns>List of RoleDto</returns>
    [HttpGet]
    public Task<IResult> ListRoles(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListRolesQuery() { PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Admin/SearchRoles?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of RoleDto</returns>
    [HttpGet]
    public Task<IResult> SearchRoles(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchRolesQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Admin/FindRole/[id]
    /// </summary>
    /// <returns>RoleDto</returns>
    [HttpGet("{id}")]
    public Task<IResult> FindRole(string id)
    {
        return _mediator.Send(new FindRoleQuery() { RoleId = id });
    }

    /// <summary>
    /// Admin/ListUsers
    /// </summary>
    /// <returns>List of UserDto</returns>
    [HttpGet]
    public Task<IResult> ListUsers(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListUsersQuery() { PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Admin/SearchUsers?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of UserDto</returns>
    [HttpGet]
    public Task<IResult> SearchUsers(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchUsersQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Admin/FindUser/[id]
    /// </summary>
    /// <returns>UserDto</returns>
    [HttpGet("{id}")]
    public Task<IResult> FindUser(string id)
    {
        return _mediator.Send(new FindUserQuery() { UserId = id });
    }

    /// <summary>
    /// Admin/CreateRole
    /// </summary>
    /// <returns>Message</returns>
    [HttpPost]
    public Task<IResult> CreateRole(CreateRoleCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Admin/AssignRoleToUsers
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> AssignRoleToUsers(AssignRoleToUsersCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Admin/UnassignRoleFromUsers
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> UnassignRoleFromUsers(UnassignRoleFromUsersCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Admin/UpdateRoleUsers
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> UpdateRoleUsers(UpdateRoleUsersCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Admin/UpdateRole
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> UpdateRole(UpdateRoleCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Admin/DeleteRole
    /// </summary>
    /// <returns>Message</returns>
    [HttpDelete("{roleId}")]
    public Task<IResult> DeleteRole(string roleId)
    {
        return _mediator.Send( new DeleteRoleCommand() { RoleId = roleId });
    }

    /// <summary>
    /// Admin/CreateUser
    /// </summary>
    /// <returns>Message</returns>
    [HttpPost]
    public Task<IResult> CreateUser(CreateUserCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Admin/AddUserToRoles
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> AddUserToRoles(AddUserToRolesCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Admin/RemoveUserFromRoles
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> RemoveUserFromRoles(RemoveUserFromRolesCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Admin/UpdateUserRoles
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> UpdateUserRoles(UpdateUserRolesCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Admin/UpdateUser
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> UpdateUser(UpdateUserCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Admin/DeleteUser
    /// </summary>
    /// <returns>Message</returns>
    [HttpDelete("{userId}")]
    public Task<IResult> DeleteUser(string userId)
    {
        return _mediator.Send( new DeleteUserCommand() { UserId = userId });
    }

    /// <summary>
    /// Admin/RevokeToken
    /// </summary>
    /// <returns>Message</returns>
    [HttpPut]
    public Task<IResult> RevokeToken(RevokeTokenCommand command)
    {
        return _mediator.Send(command);
    }

}
