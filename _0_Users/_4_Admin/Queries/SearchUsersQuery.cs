using System.Linq.Dynamic.Core;
using AutoMapper;
using MediatR;

using Common;
using DB.Common;
using DB;
using Entities;
using Dtos;

namespace Admin.Queries;

public class SearchUsersQuery : SearchQuery {}

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public SearchUsersQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper,
        ILogger<User> logger
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        SearchUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.Where is null && request.Select is null && request.OrderBy is null)
        {
            _errorMessage = "SearchUsers: Must supply at least one of the following: [where, select, orderBy]";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(new { Message = _errorMessage });
        }

        var query = _dbQueryService.GetQuery<User>();
        if (request.Where is not null)
            query = query.Where(request.Where);
        if (request.OrderBy is not null)
            query = query.OrderBy(request.OrderBy.RemoveEmptyElements(','));
        if (request.Select is not null)
            query = query.Select(request.Select.RemoveEmptyElements(',')) as IQueryable<User>;

        IResult result;
        if (request.PageSize is not null && request.PageNumber is not null)
        {
            var page = await _dbQueryService.GetPageAsync<User>(query, (int)request.PageSize, (int)request.PageNumber);
            result = request.Select is not null
                ? Results.Ok(page)
                : Results.Ok(new PageResult<UserDto>()
                    {
                        PageItems = _mapper.Map<List<UserDto>>(page.PageItems),
                        TotalItems = page.TotalItems,
                        TotalPages = page.TotalPages
                    });
        }
        else
        {
            result = request.Select is not null
                ? Results.Ok(await query.ToDynamicListAsync())
                : Results.Ok(_mapper.Map<List<UserDto>>(query.ToList()));
        }

        return result;
    }

}
