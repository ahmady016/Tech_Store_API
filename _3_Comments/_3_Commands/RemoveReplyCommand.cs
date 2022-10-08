using System.Net;
using MediatR;

using DB;
using Common;
using Entities;

namespace Replies.Commands;

public class RemoveReplyCommand : IdInput {}

public class RemoveReplyCommandHandler : IRequestHandler<RemoveReplyCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly ILogger<Reply> _logger;
    private string _errorMessage;
    public RemoveReplyCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        ILogger<Reply> logger
    )
    {
        _dbQueryService = dbQueryService;
        _dbCommandService = dbCommandService;
        _logger = logger;
    }
    public async Task<IResult> Handle(
        RemoveReplyCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedReply = await _dbQueryService.FindAsync<Reply>(command.Id);
        if (existedReply is null)
        {
            _errorMessage = $"Reply Record with Id: {command.Id} Not Found";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.NotFound);
        }

        _dbCommandService.Remove<Reply>(existedReply);
        await _dbCommandService.SaveChangesAsync();

        return Results.Ok(new { Message = $"Reply Record with Id: {command.Id} was Removed Successfully ..." });
    }
}
