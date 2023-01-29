using MediatR;
using Mapster;

using TechStoreApi.Common;
using TechStoreApi.DB.Common;
using TechStoreApi.DB;
using TechStoreApi.Entities;
using TechStoreApi.Dtos;

namespace TechStoreApi.Admin.Queries;

public class ListUsersQuery : ListQuery {}

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, IResult>
{
    private readonly IRawDBService _rawDBService;
    public ListUsersQueryHandler(IRawDBService rawDBService)
    {
        _rawDBService = rawDBService;
    }

    public async Task<IResult> Handle(
        ListUsersQuery query,
        CancellationToken cancellationToken
    )
    {
        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _rawDBService.GetPageAsync<User>(
                _rawDBService.GetQuery<User>(),
                (int)query.PageSize,
                (int)query.PageNumber
            );
            return Results.Ok(new PageResult<UserDto>()
            {
                PageItems = page.PageItems.Adapt<List<UserDto>>(),
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            });
        }
        else
        {
            var list = await _rawDBService.GetAllAsync<User>();
            return Results.Ok(list.Adapt<List<UserDto>>());
        }
    }
}
