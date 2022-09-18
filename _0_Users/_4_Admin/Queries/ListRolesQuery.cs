using MediatR;
using AutoMapper;

using Common;
using DB.Common;
using Entities;
using Dtos;

namespace Admin.Queries;
public class ListRolesQuery : ListQuery {}

public class ListRolesQueryHandler : IRequestHandler<ListRolesQuery, IResult> {
    private readonly IAdminService _adminService;
    private readonly IMapper _mapper;
    public ListRolesQueryHandler (
        IAdminService adminService,
        IMapper mapper
    )
    {
        _adminService = adminService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle (
        ListRolesQuery request,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        if (request.PageSize is not null && request.PageSize is not null)
        {
            var page = await _adminService.GetPageAsync<Role>(_adminService.GetQuery<Role>(), (int)request.PageSize, (int)request.PageNumber);
            result = Results.Ok(new PageResult<RoleDto>()
            {
                PageItems = _mapper.Map<List<RoleDto>>(page.PageItems),
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            });
        }
        else
        {
            var list = _adminService.GetAllAsync<Role>();
            result = Results.Ok(_mapper.Map<List<RoleDto>>(list));
        }

        return result;
    }

}
