using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MediatR;

using Entities;
using Dtos;

namespace Admin.Queries;

public class FindRoleQuery : IRequest<IResult>
{
    [Required(ErrorMessage = "RoleId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "RoleId Must between 10 and 450 characters")]
    public string RoleId { get; set; }
}

public class FindRoleQueryHandler : IRequestHandler<FindRoleQuery, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<Role> _logger;
    private string _errorMessage;
    public FindRoleQueryHandler (
        RoleManager<Role> roleManager,
        UserManager<User> userManager,
        IMapper mapper,
        ILogger<Role> logger
    )
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        FindRoleQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedRole = await _roleManager.FindByIdAsync(request.RoleId);
        if(existedRole is null)
        {
            _errorMessage = $"Role Record with Id: {request.RoleId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }
        var roleUsers = await _userManager.GetUsersInRoleAsync(existedRole.Name);

        var role = _mapper.Map<RoleDto>(existedRole);
        role.Users = _mapper.Map<List<UserDto>>(roleUsers);
        return Results.Ok(role);
    }

}
