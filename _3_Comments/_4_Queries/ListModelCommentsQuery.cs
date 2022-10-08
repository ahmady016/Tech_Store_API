using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using DB.Common;
using Entities;
using Dtos;

namespace Comments.Queries;

public class ListModelCommentsQuery : IRequest<IResult>
{
    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }
    public int? PageSize { get; set; } = null;
    public int? PageNumber { get; set; } = null;
}

public class ListModelCommentsQueryHandler : IRequestHandler<ListModelCommentsQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public ListModelCommentsQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        ListModelCommentsQuery query,
        CancellationToken cancellationToken
    )
    {
        Expression<Func<Comment, bool>> where = e => e.ModelId == query.ModelId;
        IResult result;
        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _dbQueryService.GetPageAsync<Comment>(
                _dbQueryService.GetQuery<Comment>(where),
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
            var list = await _dbQueryService.GetListAsync<Comment>(where);
            result = Results.Ok(_mapper.Map<List<CommentDto>>(list));
        }
        return result;
    }
}
