using Microsoft.AspNetCore.Mvc;
using MediatR;

using TechStoreApi.Admin.Commands;
using TechStoreApi.Admin.Queries;

namespace TechStoreApi.Admin;

[ApiController]
[Route("api/[controller]/[action]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminController(IMediator mediator) => _mediator = mediator;

    #region User Queries
    [HttpGet]
    public Task<IResult> ListUsers([FromQuery][AsParameters] ListUsersQuery query)
    {
        return _mediator.Send(query);
    }
    [HttpGet]
    public Task<IResult> SearchUsers([FromQuery][AsParameters] SearchUsersQuery query)
    {
        return _mediator.Send(query);
    }
    [HttpGet("{id}")]
    public Task<IResult> FindUser([FromRoute][AsParameters] FindUserQuery query)
    {
        return _mediator.Send(query);
    }
    #endregion

    #region Role Queries
    [HttpGet]
    public Task<IResult> ListRoles([FromQuery][AsParameters] ListRolesQuery query)
    {
        return _mediator.Send(query);
    }
    [HttpGet]
    public Task<IResult> SearchRoles([FromQuery][AsParameters] SearchRolesQuery query)
    {
        return _mediator.Send(query);
    }
    [HttpGet("{id}")]
    public Task<IResult> FindRole([FromRoute][AsParameters] FindRoleQuery query)
    {
        return _mediator.Send(query);
    }
    #endregion

    #region User Commands
    [HttpPost]
    public Task<IResult> CreateUser(CreateUserCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpPut]
    public Task<IResult> UpdateUser(UpdateUserCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpDelete("{id}")]
    public Task<IResult> DeleteUser([FromRoute][AsParameters] DeleteUserCommand command)
    {
        return _mediator.Send(command);
    }
    #endregion

    #region Role Commands
    [HttpPost]
    public Task<IResult> CreateRole(CreateRoleCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpPut]
    public Task<IResult> UpdateRole(UpdateRoleCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpDelete("{id}")]
    public Task<IResult> DeleteRole([FromRoute][AsParameters] DeleteRoleCommand command)
    {
        return _mediator.Send(command);
    }
    #endregion

    #region UserRole Commands
    [HttpPost]
    public Task<IResult> AddUserToRoles(AddUserToRolesCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpPost]
    public Task<IResult> RemoveUserFromRoles(RemoveUserFromRolesCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpPut]
    public Task<IResult> UpdateUserRoles(UpdateUserRolesCommand command)
    {
        return _mediator.Send(command);
    }

    [HttpPost]
    public Task<IResult> AssignRoleToUsers(AssignRoleToUsersCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpPost]
    public Task<IResult> UnassignRoleFromUsers(UnassignRoleFromUsersCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpPut]
    public Task<IResult> UpdateRoleUsers(UpdateRoleUsersCommand command)
    {
        return _mediator.Send(command);
    }
    #endregion

    #region Other Commands
    [HttpPost]
    public Task<IResult> SendConfirmationEmail(ResendConfirmationEmailCommand command)
    {
        return _mediator.Send(command);
    }
    [HttpPut]
    public Task<IResult> RevokeRefreshToken(RevokeTokenCommand command)
    {
        return _mediator.Send(command);
    }
    #endregion
}
