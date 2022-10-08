using AutoMapper;
using MediatR;

using DB;
using Common;
using DB.Common;
using Entities;
using Dtos;

namespace Comments.Queries;

public class ListAllCommentsQuery : ListQuery {}

public class ListAllCommentsQueryHandler : IRequestHandler<ListAllCommentsQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public ListAllCommentsQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        ListAllCommentsQuery query,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _dbQueryService.GetPageAsync<Comment>(
                _dbQueryService.GetQuery<Comment>(),
                (int)query.PageSize,
                (int)query.PageNumber
            );
            result = Results.Ok(new PageResult<CommentDto>()
            {
                PageItems = _mapper.Map<List<CommentDto>>(page.PageItems),
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            });
        }
        else
        {
            var list = await _dbQueryService.GetAllAsync<Comment>();
            result = Results.Ok(_mapper.Map<List<CommentDto>>(list));
        }
        return result;
    }
}
