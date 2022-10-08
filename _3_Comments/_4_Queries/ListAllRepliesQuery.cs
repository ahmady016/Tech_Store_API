using AutoMapper;
using MediatR;

using DB;
using Common;
using DB.Common;
using Entities;
using Dtos;

namespace Replies.Queries;

public class ListAllRepliesQuery : ListQuery {}

public class ListAllRepliesQueryHandler : IRequestHandler<ListAllRepliesQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public ListAllRepliesQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        ListAllRepliesQuery query,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _dbQueryService.GetPageAsync<Reply>(
                _dbQueryService.GetQuery<Reply>(),
                (int)query.PageSize,
                (int)query.PageNumber
            );
            result = Results.Ok(new PageResult<ReplyDto>()
            {
                PageItems = _mapper.Map<List<ReplyDto>>(page.PageItems),
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            });
        }
        else
        {
            var list = await _dbQueryService.GetAllAsync<Reply>();
            result = Results.Ok(_mapper.Map<List<ReplyDto>>(list));
        }
        return result;
    }
}
