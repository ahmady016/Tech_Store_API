using MediatR;
using AutoMapper;

using Common;
using DB.Common;
using DB;
using Entities;
using Dtos;

namespace Admin.Queries;

public class ListUsersQuery : ListQuery {}

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, IResult> {
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public ListUsersQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle (
        ListUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        if (request.PageSize is not null && request.PageNumber is not null)
        {
            var page = await _dbQueryService.GetPageAsync<User>(
                _dbQueryService.GetQuery<User>(),
                (int)request.PageSize,
                (int)request.PageNumber
            );
            result = Results.Ok(new PageResult<UserDto>()
            {
                PageItems = _mapper.Map<List<UserDto>>(page.PageItems),
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            });
        }
        else
        {
            var list = _dbQueryService.GetAllAsync<User>();
            result = Results.Ok(_mapper.Map<List<UserDto>>(list));
        }

        return result;
    }

}
