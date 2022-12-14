using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MediatR;

using DB;
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
    private readonly UserManager<User> _userManager;
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public FindUserQueryHandler (
        UserManager<User> userManager,
        IDBQueryService dbQueryService,
        IMapper mapper,
        ILogger<User> logger
    )
    {
        _userManager = userManager;
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        FindUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedUser = await _userManager.FindByIdAsync(request.UserId);
        if(existedUser is null)
        {
            _errorMessage = $"User Record with Id: {request.UserId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        existedUser.Roles = await _dbQueryService.GetQuery<UserRole>()
            .Include(e => e.Role)
            .Where(e => e.UserId == request.UserId)
            .ToListAsync();

        var userDto = _mapper.Map<UserDto>(existedUser);
        userDto.Roles = _mapper.Map<List<RoleDto>>(existedUser.Roles.Select(e => e.Role));
        return Results.Ok(userDto);
    }

}
