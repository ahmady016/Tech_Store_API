using MediatR;
using Mapster;

using TechStoreApi.Common;
using TechStoreApi.DB.Common;
using TechStoreApi.DB;
using TechStoreApi.Entities;
using TechStoreApi.Dtos;

namespace TechStoreApi.Admin.Queries;

public class ListRolesQuery : ListQuery {}

public class ListRolesQueryHandler : IRequestHandler<ListRolesQuery, IResult>
{
    private readonly IRawDBService _rawDBService;
    public ListRolesQueryHandler(IRawDBService rawDBService)
    {
        _rawDBService = rawDBService;
    }

    public async Task<IResult> Handle(
        ListRolesQuery query,
        CancellationToken cancellationToken
    )
    {
        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _rawDBService.GetPageAsync<Role>(
                _rawDBService.GetQuery<Role>(),
                (int)query.PageSize,
                (int)query.PageNumber
            );
            return Results.Ok(new PageResult<RoleDto>()
            {
                PageItems = page.PageItems.Adapt<List<RoleDto>>(),
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            });
        }
        else
        {
            var list = await _rawDBService.GetAllAsync<Role>();
            return Results.Ok(list.Adapt<List<RoleDto>>());
        }
    }
}
