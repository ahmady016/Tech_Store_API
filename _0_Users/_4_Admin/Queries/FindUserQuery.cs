using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using Entities;
using Dtos;

namespace Admin.Queries;

public class FindUserQuery : IRequest<IResult>
{
    [Required(ErrorMessage = "UserId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "UserId Must between 10 and 450 characters")]
    public string UserId { get; set; }
}

public class FindUserQueryHandler : IRequestHandler<FindUserQuery, IResult>
{
    private readonly IAdminService _adminService;
    private readonly IMapper _mapper;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public FindUserQueryHandler (
        IAdminService adminService,
        IMapper mapper,
        ILogger<User> logger
    )
    {
        _adminService = adminService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        FindUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedUser = await _adminService.FindAsync<User>(request.UserId);
        if(existedUser is null)
        {
            _errorMessage = $"User Record with Id: {request.UserId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        var usersAndRoles = await _adminService.GetQuery<UserRole>()
            .Include(e => e.Role)
            .Where(e => e.UserId == request.UserId)
            .ToListAsync();

        var user = _mapper.Map<UserDto>(existedUser);
        user.Roles = _mapper.Map<List<RoleDto>>(usersAndRoles.Select(e => e.Role));
        return Results.Ok(user);
    }

}
