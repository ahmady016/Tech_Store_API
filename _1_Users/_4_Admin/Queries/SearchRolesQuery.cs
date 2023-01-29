using System.Linq.Dynamic.Core;
using MediatR;
using Mapster;

using TechStoreApi.Common;
using TechStoreApi.DB.Common;
using TechStoreApi.DB;
using TechStoreApi.Entities;
using TechStoreApi.Dtos;

namespace TechStoreApi.Admin.Queries;

public class SearchRolesQuery : SearchQuery {}

public class SearchRolesQueryHandler : IRequestHandler<SearchRolesQuery, IResult>
{
    private readonly IRawDBService _rawDBService;
    private readonly IResultService _resultService;
    public SearchRolesQueryHandler(
        IRawDBService rawDBService,
        IResultService resultService
    )
    {
        _rawDBService = rawDBService;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        SearchRolesQuery query,
        CancellationToken cancellationToken
    )
    {
        if (query.Where is null && query.Select is null && query.OrderBy is null)
            return _resultService.ParameterlessSearch(nameof(SearchRolesQuery));

        var dbQuery = _rawDBService.GetQuery<Role>() as IQueryable;
        if (query.Where is not null)
            dbQuery = dbQuery.Where(query.Where);
        if (query.OrderBy is not null)
            dbQuery = dbQuery.OrderBy(query.OrderBy);
        if (query.Select is not null)
            dbQuery = dbQuery.Select(query.Select);

        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _rawDBService.GetPageAsync(dbQuery, (int)query.PageSize, (int)query.PageNumber);
            return query.Select is not null
                ? Results.Ok(page)
                : Results.Ok(new PageResult<RoleDto>()
                    {
                        PageItems = page.PageItems.Adapt<List<RoleDto>>(),
                        TotalItems = page.TotalItems,
                        TotalPages = page.TotalPages
                    });
        }
        else
        {
            var list = await dbQuery.ToDynamicListAsync();
            return query.Select is not null
                ? Results.Ok(list)
                : Results.Ok(list.Adapt<List<RoleDto>>());
        }
    }
}
