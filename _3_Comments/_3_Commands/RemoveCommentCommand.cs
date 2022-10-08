using System.Net;
using MediatR;

using DB;
using Common;
using Entities;

namespace Comments.Commands;

public class RemoveCommentCommand : IdInput {}

public class RemoveCommentCommandHandler : IRequestHandler<RemoveCommentCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly ILogger<Comment> _logger;
    private string _errorMessage;
    public RemoveCommentCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        ILogger<Comment> logger
    )
    {
        _dbQueryService = dbQueryService;
        _dbCommandService = dbCommandService;
        _logger = logger;
    }
    public async Task<IResult> Handle(
        RemoveCommentCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedComment = await _dbQueryService.FindAsync<Comment>(command.Id);
        if (existedComment is null)
        {
            _errorMessage = $"Comment Record with Id: {command.Id} Not Found";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.NotFound);
        }

        _dbCommandService.Remove<Comment>(existedComment);
        await _dbCommandService.SaveChangesAsync();

        return Results.Ok(new { Message = $"Comment Record with Id: {command.Id} was Removed Successfully ..." });
    }
}
