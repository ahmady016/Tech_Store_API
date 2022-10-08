using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;
using Dtos;

namespace Comments.Queries;

public class FindCommentWithRepliesQuery : IdInput {}

public class FindCommentWithRepliesQueryHandler : IRequestHandler<FindCommentWithRepliesQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    private readonly ILogger<Comment> _logger;
    private string _errorMessage;
    public FindCommentWithRepliesQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper,
        ILogger<Comment> logger
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        FindCommentWithRepliesQuery query,
        CancellationToken cancellationToken
    )
    {
        var existedComment = await _dbQueryService.GetQuery<Comment>()
            .Include(e => e.Replies)
            .Where(e => e.Id == query.Id)
            .FirstOrDefaultAsync();

        if(existedComment is null)
        {
            _errorMessage = $"Comment Record with Id: {query.Id} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        return Results.Ok(_mapper.Map<CommentDto>(existedComment));
    }

}
