using MediatR;
using AutoMapper;

using Common;
using DB.Common;
using Entities;
using Dtos;

namespace Admin.Queries;

public class ListUsersQuery : ListQuery {}

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, IResult> {
    private readonly IAdminService _adminService;
    private readonly IMapper _mapper;
    public ListUsersQueryHandler (
        IAdminService adminService,
        IMapper mapper
    )
    {
        _adminService = adminService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle (
        ListUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        if (request.PageSize is not null && request.PageSize is not null)
        {
            var page = await _adminService.GetPageAsync<User>(_adminService.GetQuery<User>(), (int)request.PageSize, (int)request.PageNumber);
            result = Results.Ok(new PageResult<UserDto>()
            {
                PageItems = _mapper.Map<List<UserDto>>(page.PageItems),
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            });
        }
        else
        {
            var list = _adminService.GetAllAsync<User>();
            result = Results.Ok(_mapper.Map<List<UserDto>>(list));
        }

        return result;
    }

}
